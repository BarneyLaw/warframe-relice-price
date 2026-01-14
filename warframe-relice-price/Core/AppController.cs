using System;
using System.Windows;
using System.Windows.Threading;

using warframe_relice_price.OCRVision;
using warframe_relice_price.OverlayUI;
using warframe_relice_price.Utils;
using warframe_relice_price.WarframeTracker;

using Rewards.Processing;

namespace warframe_relice_price.Core
{
    class AppController 
    {
        private readonly MainWindow _window;
        private readonly OverlayRenderer _overlayRenderer;
        private readonly WarframeWindowTracker _tracker;

        private IntPtr _warframeHwnd;
        private AppState _state = AppState.Idle;

        private DateTime _lastDetectionOcrAtUtc = DateTime.MinValue;
        private readonly TimeSpan _detectionOcrInterval = TimeSpan.FromMilliseconds(750);

        private int _rewardScreenMisses;
        private const int RewardScreenMissesToReset = 4;

        private DateTime _rewardScreenEnteredAtUtc;
        private readonly TimeSpan _minRewardScreenTime = TimeSpan.FromSeconds(2);
        private readonly TimeSpan _maxRewardScreenTime = TimeSpan.FromSeconds(15);

        // To track stable reward capture
        private int _rewardDetectionStreak;
        private const int RewardDetectionStreakRequired = 4;

        // To delay capturing stable reward after detection
        private bool _hasCapturedStableReward;
        private readonly TimeSpan _rewardOcrDelay = TimeSpan.FromMilliseconds(1000);

        private List<int?> _prices = new List<int?>();

        public AppController(MainWindow window) 
        { 
            _window = window;
            _overlayRenderer = new OverlayRenderer(window.OverlayCanvas);
            _tracker = new WarframeWindowTracker();

            WarframeProcess.WarframeStarted += OnWarframeStarted;
            WarframeProcess.WarframeStopped += OnWarframeStopped;

            WarframeProcess.TryUpdateFromPolling();

            if (WarframeProcess.checkIfRunning())
            {
                _window.Show();
                _state = AppState.InWarframe;
            }
            else
            {
                _window.Hide();
                _state = AppState.Idle;
            }
        }

        // Starts the main loop to track Warframe window and update overlay
        public void startLoop()
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(250)
            };
            timer.Tick += OnTick;
            timer.Start();
        }
        private void OnWarframeStarted(int pid)
        {
            _warframeHwnd = IntPtr.Zero;
            _window.Show();
            _hasCapturedStableReward = false;
        }

        private void OnWarframeStopped(int pid)
        {
            _warframeHwnd = IntPtr.Zero;
            _window.Hide();
            _window.OverlayCanvas.Children.Clear();
            _state = AppState.Idle;
            _hasCapturedStableReward = false;
        }

        private void OnTick(object sender, EventArgs e)
        {
            // Update PID + raise WarframeStarted/WarframeStopped events based on polling.
            if (WarframeProcess.TryUpdateFromPolling())
            {
                // If the process state changed, force reacquire handle.
                _warframeHwnd = IntPtr.Zero;
            }

            if (_warframeHwnd == IntPtr.Zero)
            {
                _warframeHwnd = _tracker.GetWarframeWindow();
            }

            if (_warframeHwnd == IntPtr.Zero)
            {
                _state = AppState.Idle;
                return;
            }

            // Check if Warframe is the foreground window
            //if (Win32.GetForegroundWindow() != _warframeHwnd)
            //{
            //    _window.Hide();
            //    return;
            //}
            //else
            //{
            //    _window.Show();
            //}

            if (_tracker.TryGetBounds(_warframeHwnd, out var rect))
            {
                _window.Left = rect.Left;
                _window.Top = rect.Top;
                _window.Width = rect.Right - rect.Left;
                _window.Height = rect.Bottom - rect.Top;

                if (_state == AppState.Idle)
                {
                    _state = AppState.InWarframe;
                }

                if (_state == AppState.InWarframe)
                {
                    // Rate-limit the expensive OCR call.
                    if (DateTime.UtcNow - _lastDetectionOcrAtUtc < _detectionOcrInterval)
                    {
                        return;
                    }

                    _lastDetectionOcrAtUtc = DateTime.UtcNow;

                    if (CheckForRewardScreen.TryDetectRewardScreen(out var detectionText))
                    {
                        Logger.Log($"Reward screen detected. OCR(detection box)='{detectionText}'");

                        _rewardDetectionStreak++;

                        if (_rewardDetectionStreak >= RewardDetectionStreakRequired)
                        {
                            Logger.Log($"Reward screen confirmed. OCR='{detectionText}'");

                            _state = AppState.Reward;
                            _rewardScreenEnteredAtUtc = DateTime.UtcNow;
                            _hasCapturedStableReward = false;
                            _rewardScreenMisses = 0;
                            _rewardDetectionStreak = 0;
                        }
                        else
                        {
                            Logger.Log($"Reward screen detection streak: {_rewardDetectionStreak}/{RewardDetectionStreakRequired}");
                        }
                    }
                    else
                    {
                        _rewardDetectionStreak = 0;
                    }
                }
                else if (_state == AppState.Reward)
                {
                    var rewardAge = DateTime.UtcNow - _rewardScreenEnteredAtUtc;

                    // --- Phase 1: wait for UI to stabilize ---
                    if (!_hasCapturedStableReward)
                    {
                        if (rewardAge >= _rewardOcrDelay)
                        {
                            captureStableReward();
                            _hasCapturedStableReward = true;
                        }
                        return;
                    }

                    // --- Phase 2: detect reward screen disappearance ---
                    if (DateTime.UtcNow - _lastDetectionOcrAtUtc < _detectionOcrInterval)
                        return;

                    _lastDetectionOcrAtUtc = DateTime.UtcNow;

                    if (CheckForRewardScreen.TryDetectRewardScreen(out _))
                    {
                        _overlayRenderer.DrawRelicPrices(_prices);
                        _rewardScreenMisses = 0;
                        return;
                    }

                    _rewardScreenMisses++;

                    // Ignore misses too early
                    if (rewardAge < _minRewardScreenTime)
                        return;

                    // Hard safety timeout
                    if (rewardAge > _maxRewardScreenTime ||
                        _rewardScreenMisses >= RewardScreenMissesToReset)
                    {
                        Logger.Log("Reward screen ended — returning to InWarframe");
                        ResetToInWarframe();
                    }
                }
                //_overlayRenderer.DrawTestBoundary();
                //_overlayRenderer.DrawAll(_window.Width, _window.Height, 4);
            }
        }

        private void ResetToInWarframe()
        {
            _prices.Clear();
            _state = AppState.InWarframe;
            _hasCapturedStableReward = false;
            _rewardScreenMisses = 0;
        }

        private void captureStableReward()
        {
            var screenRowRect = ScreenCaptureRow.ToScreenRect(ScreenCaptureRow.row_rect);
            using var bmp = ScreenCaptureRow.captureRegion(screenRowRect);
            string rowText = ImageToText.multiPassOCR(bmp);

            // Maybe we can use another method to count rewards here?
            // int numRewards = RewardCounter.Count(rowText).Count;
            int numRewards = CheckForRewardScreen.CountRewards();
            Logger.Log($"Capturing stable reward with {numRewards} rewards.");
            
            List<int?> prices = new List<int?>();

            for (int i = 0; i < numRewards; i++)
            {
                int? price = RewardPrice.getPriceForItem(i, numRewards);
                _prices.Add(price);
            }
            Logger.Log($"Captured {_prices.Count} prices.");

        }
    }
}
