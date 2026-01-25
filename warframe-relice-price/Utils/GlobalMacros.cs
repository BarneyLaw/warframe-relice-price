using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace warframe_relice_price.Utils
{
    internal class GlobalMacros : IDisposable
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private readonly int _id;
        private readonly nint _hWnd;
        private readonly HwndSource _source;

        public event Action? Pressed;

        public GlobalMacros(WindowInteropHelper host, HotKeyModifiers modifiers, uint key)
        {
            _hWnd = host.Handle;
            _id = GetHashCode();
            _source = HwndSource.FromHwnd(_hWnd) ?? throw new InvalidOperationException("Could not get HwndSource");

            if (!RegisterHotKey(_hWnd, _id, (uint)modifiers, key))
            {
                throw new InvalidOperationException("Could not register hot key");
            }

            _source.AddHook(WndProc);
        }

        public void Dispose()
        {
            UnregisterHotKey(_hWnd, _id);
            _source.RemoveHook(WndProc);
        }

        private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            if (msg == WM_HOTKEY && wParam.ToInt32() == _id)
            {
                Pressed?.Invoke();
                handled = true;
            }

            return 0;
        }

        [Flags]
        public enum HotKeyModifiers : uint
        {
            None = 0x0000,
            Alt = 0x0001,
            Control = 0x0002,
            Shift = 0x0004,
            Win = 0x0008,
            NoRepeat = 0x4000
        }
    }
}
