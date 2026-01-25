using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using warframe_relice_price.Utils;

namespace warframe_relice_price.OverlayUI
{
    internal sealed class OverlayShellController : IDisposable
    {
        private readonly MainWindow _window;
        private readonly OverlayRenderer _renderer;

        private GlobalMacros? _toggleHotKey;
        private nint _hwnd;
        private bool _isActive;
        public bool IsMenuOpen => _renderer.IsOverlayMenuOpen;

        public OverlayShellController(MainWindow window, OverlayRenderer renderer)
        {
            _window = window;
            _renderer = renderer;

            _window.SourceInitialized += (_, _) =>
            {
                _hwnd = new WindowInteropHelper(_window).Handle;
            };

            _window.PreviewKeyDown += OnPreviewKeyDown;
        }

        public void Dispose()
        {
            DisarmHotKey();
            _window.PreviewKeyDown -= OnPreviewKeyDown;
        }

        public void SetOverlayActive(bool active)
        {
            if (_isActive == active)
                return;

            _isActive = active;

            if (!_isActive)
            {
                _renderer.Menu.CloseOverlayMenuIfOpen();
                DisarmHotKey();
                _window.Hide();
                return;
            }

            _window.Show();
            ArmHotKeyIfPossible();

            // Start in click-through HUD mode
            SetClickThrough(clickThrough: true);
        }

        /// <summary>
        /// Call from your main tick after window size/position updates.
        /// Keeps the menu/dim centered.
        /// </summary>
        public void TickLayout()
        {
            _renderer.Menu.UpdateOverlayMenuLayout();
        }

        private void ArmHotKeyIfPossible()
        {
            if (_toggleHotKey != null)
                return;

            if (_hwnd == 0)
                _hwnd = new WindowInteropHelper(_window).Handle;

            if (_hwnd == 0)
                return;

            _toggleHotKey = new GlobalMacros(
                new WindowInteropHelper(_window),
                GlobalMacros.HotKeyModifiers.Shift | GlobalMacros.HotKeyModifiers.NoRepeat,
                key: 0x78 /* VK_F9 */);

            _toggleHotKey.Pressed += ToggleMenu;
        }

        private void DisarmHotKey()
        {
            if (_toggleHotKey == null)
                return;

            _toggleHotKey.Pressed -= ToggleMenu;
            _toggleHotKey.Dispose();
            _toggleHotKey = null;
        }

        private void ToggleMenu()
        {
            if (!_isActive)
                return;

            if (_renderer.IsOverlayMenuOpen)
            {
                _renderer.Menu.CloseOverlayMenuIfOpen();
                SetClickThrough(clickThrough: true);
                return;
            }

            _renderer.Menu.OpenOverlayMenu();
            _renderer.Menu.UpdateOverlayMenuLayout();

            // Make overlay interactive while menu is opened
            SetClickThrough(clickThrough: false);

            // Optional: steal focus so X/Esc/Q works even if Warframe had focus.
            _window.Activate();
            _window.Focus();
            Keyboard.Focus(_window);
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!_isActive || !_renderer.IsOverlayMenuOpen)
                return;

            if (e.Key == Key.X || e.Key == Key.Escape)
            {
                ToggleMenu();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Q)
            {
                Application.Current.Shutdown();
                e.Handled = true;
            }
        }

        private void SetClickThrough(bool clickThrough)
        {
            if (_hwnd == 0)
                return;

            int exStyle = Win32.GetWindowLong(_hwnd, Win32.GWL_EXSTYLE);

            nint newStyle = clickThrough
                ? (nint)(exStyle | Win32.WS_EX_LAYERED | Win32.WS_EX_TRANSPARENT)
                : (nint)((exStyle | Win32.WS_EX_LAYERED) & ~Win32.WS_EX_TRANSPARENT);

            Win32.SetWindowLong(_hwnd, Win32.GWL_EXSTYLE, newStyle);
        }
    }
}