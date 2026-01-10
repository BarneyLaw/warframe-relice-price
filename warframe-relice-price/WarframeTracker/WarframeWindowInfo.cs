using System;
using warframe_relice_price.Utils;

namespace warframe_relice_price.WarframeTracker
{
    public static class WarframeWindowInfo
    {
        public static int XOffset => GetRect().Left;
        public static int YOffset => GetRect().Top;
        public static int Width => GetRect().Right - GetRect().Left;
        public static int Height => GetRect().Bottom - GetRect().Top;

        public static Win32.RECT GetRect()
        {
            var tracker = new WarframeWindowTracker();
            var hwnd = tracker.GetWarframeWindow();
            if (hwnd != IntPtr.Zero && tracker.TryGetBounds(hwnd, out var rect))
                return rect;
            // Fallback: return a default rect if not found
            return new Win32.RECT { Left = 0, Top = 0, Right = 0, Bottom = 0 };
        }
    }
}