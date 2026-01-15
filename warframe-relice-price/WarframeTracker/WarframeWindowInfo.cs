using System;
using warframe_relice_price.Utils;

namespace warframe_relice_price.WarframeTracker
{
    public static class WarframeWindowInfo
    {
        // WPF (DIPs)
        public static double WidthDip { get; private set; }
        public static double HeightDip { get; private set; }
        public static double XOffsetDip { get; private set; }
        public static double YOffsetDip { get; private set; }

        // Physical pixels (for screen capture)
        public static int WidthPx { get; private set; }
        public static int HeightPx { get; private set; }
        public static int XOffsetPx { get; private set; }
        public static int YOffsetPx { get; private set; }

        // DPI scale (pixels per DIP)
        public static double DpiX { get; private set; }
        public static double DpiY { get; private set; }

        public static void UpdateFromRect(Win32.RECT rect, double dpiX, double dpiY)
        {
            WidthPx = rect.Right - rect.Left;
            HeightPx = rect.Bottom - rect.Top;
            XOffsetPx = rect.Left;
            YOffsetPx = rect.Top;

            DpiX = dpiX;
            DpiY = dpiY;

            WidthDip = WidthPx / dpiX;
            HeightDip = HeightPx / dpiY;
            XOffsetDip = rect.Left / dpiX;
            YOffsetDip = rect.Top / dpiY;
        }
    }
}