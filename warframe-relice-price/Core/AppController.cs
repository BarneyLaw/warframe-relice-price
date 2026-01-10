using System;
using System.Windows;
using System.Windows.Threading;

using warframe_relice_price.OCRVision;
using warframe_relice_price.OverlayUI;
using warframe_relice_price.Utils;
using warframe_relice_price.WarframeTracker;

namespace warframe_relice_price.Core
{
    class AppController
    {
        private readonly MainWindow _window;
        private readonly OverlayRenderer _overlayRenderer;
        private readonly WarframeWindowTracker _tracker;

        private IntPtr _warframeHwnd;
        private AppState _state = AppState.Idle;
        private bool _hasLoggedRewardRowText;

        private DateTime _lastDetectionOcrAtUtc = DateTime.MinValue;
        private readonly TimeSpan _detectionOcrInterval = TimeSpan.FromMilliseconds(750);

        private int _rewardScreenMisses;
        private const int RewardScreenMissesToReset = 4;

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
            _hasLoggedRewardRowText = false;
        }

        private void OnWarframeStopped(int pid)
        {
            _warframeHwnd = IntPtr.Zero;
            _window.Hide();
            _window.OverlayCanvas.Children.Clear();
            _state = AppState.Idle;
            _hasLoggedRewardRowText = false;
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

                //_overlayRenderer.DrawFakeRelicPrices(_window.Width, _window.Height, 4);
                _overlayRenderer.DrawTestBoundary();

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

                        _state = AppState.Reward;
                        _hasLoggedRewardRowText = false;
                        _rewardScreenMisses = 0;
                    }
                }
                else if (_state == AppState.Reward && !_hasLoggedRewardRowText)
                {
                    // OCR the reward row exactly once per reward screen.
                    if (!_hasLoggedRewardRowText)
                    {
                        var screenRowRect = ScreenCaptureRow.ToScreenRect(ScreenCaptureRow.row_rect);
                        using var bmp = ScreenCaptureRow.captureRegion(screenRowRect);
                        string rowText = ImageToText.ConvertImageToText(bmp);

                        Logger.Log($"OCR(reward row)='{rowText}'");

                        _hasLoggedRewardRowText = true;
                    }

                    // Now watch for the reward screen to disappear to reset for the next mission.
                    // This uses the same detection box OCR, rate-limited, so it doesn't spam.
                    if (DateTime.UtcNow - _lastDetectionOcrAtUtc < _detectionOcrInterval)
                    {
                        return;
                    }

                    _lastDetectionOcrAtUtc = DateTime.UtcNow;

                    if (CheckForRewardScreen.TryDetectRewardScreen(out _))
                    {
                        _rewardScreenMisses = 0;
                    }
                    else
                    {
                        _rewardScreenMisses++;
                        if (_rewardScreenMisses >= RewardScreenMissesToReset)
                        {
                            Logger.Log("Reward screen no longer detected; returning to InWarframe.");
                            _state = AppState.InWarframe;
                            _hasLoggedRewardRowText = false;
                            _rewardScreenMisses = 0;
                        }
                    }
                }
                //_overlayRenderer.DrawTestBoundary();
                //_overlayRenderer.DrawAll(_window.Width, _window.Height, 4);
            }
        }
    }
}
