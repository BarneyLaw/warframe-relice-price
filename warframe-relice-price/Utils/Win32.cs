using System;
using System.Runtime.InteropServices;


namespace warframe_relice_price.Utils
{
    static class Win32
    {
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int WS_EX_LAYERED = 0x00080000;

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
        private static extern nint GetWindowLongPtr(nint hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
        private static extern nint SetWindowLongPtr(nint hWnd, int nIndex, nint dwNewLong);


        public static int GetWindowLong(nint hWnd, int nIndex)
        {
            return unchecked((int)GetWindowLongPtr(hWnd, nIndex));
        }

        public static nint SetWindowLong(nint hWnd, int nIndex, nint dwNewLong)
        {
            return SetWindowLongPtr(hWnd, nIndex, dwNewLong);
        }

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
