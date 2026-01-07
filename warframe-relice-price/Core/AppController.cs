using System;
using System.Windows;
using System.Windows.Threading;

using warframe_relice_price.OverlayUI;
using warframe_relice_price.WarframeTracker;

namespace warframe_relice_price.Core
{
    class AppController
    {
        private readonly MainWindow _window;
        private readonly OverlayRenderer _overlayRenderer;
        private readonly WarframeWindowTracker _tracker;

        private IntPtr _warframeHwnd;

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
            }
            else
            {
                _window.Hide();
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
        }

        private void OnWarframeStopped(int pid)
        {
            _warframeHwnd = IntPtr.Zero;
            _window.Hide();
            _window.OverlayCanvas.Children.Clear();
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
                return;
            }

            if (_tracker.TryGetBounds(_warframeHwnd, out var rect))
            {
                _window.Left = rect.Left;
                _window.Top = rect.Top;
                _window.Width = rect.Right - rect.Left;
                _window.Height = rect.Bottom - rect.Top;

                _overlayRenderer.DrawFakeRelicPrices(_window.Width, _window.Height, 4);
            }
        }
    }
}
