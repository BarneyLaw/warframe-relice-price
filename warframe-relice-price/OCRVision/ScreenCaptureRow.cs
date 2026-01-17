using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.IO;
using warframe_relice_price.Utils;

using warframe_relice_price.WarframeTracker;

namespace warframe_relice_price.OCRVision
{
    public static class ScreenCaptureRow
    {

        // Dimensions for the Detection Box that determines Reward Screen Status
        public static Rectangle GetDetectionBoxPx()
        {
            return new Rectangle(
                (int)(0.0729 * WarframeWindowInfo.WidthPx),
                (int)(0.0324 * WarframeWindowInfo.HeightPx),
                (int)(0.401 * WarframeWindowInfo.WidthPx),
                (int)(0.0602 * WarframeWindowInfo.HeightPx)
            );
        }

        // Dimensions for the whole Reward row
        public static Rectangle GetRewardRowPx()
        {
            return new Rectangle(
                (int)(0.20 * WarframeWindowInfo.WidthPx),
                (int)(0.379 * WarframeWindowInfo.HeightPx),
                (int)(0.55 * WarframeWindowInfo.WidthPx),
                (int)(0.056 * WarframeWindowInfo.HeightPx)
            );
        }

        // Dimensions for invidual boxes (n = 1 to 4)
        public static Rectangle GetRewardBoxPx(int index, int totalRewards)
        {
            const int gapPx = 5;

            int boxWidth = (int)(0.125 * WarframeWindowInfo.WidthPx);
            int boxHeight = (int)(0.056 * WarframeWindowInfo.HeightPx);

            int totalWidth =
                totalRewards * boxWidth +
                (totalRewards - 1) * gapPx;

            int startX = (WarframeWindowInfo.WidthPx - totalWidth) / 2;

            return new Rectangle(
                startX + (index - 1) * (boxWidth + gapPx),
                (int)(0.379 * WarframeWindowInfo.HeightPx),
                boxWidth,
                boxHeight
            );
        }

        public static Rectangle ToScreenRect(Rectangle pxRect)
        {
            return new Rectangle(
                WarframeWindowInfo.XOffsetPx + pxRect.X,
                WarframeWindowInfo.YOffsetPx + pxRect.Y,
                pxRect.Width,
                pxRect.Height
            );
        }
        /// <summary>
        /// Captures an image of a region on screen.
        /// </summary>
        /// <param name="region">The rectangle boundary of screen to capture</param>

        /// <returns>The Bitmap(bytes) of that region.</returns>
        public static Bitmap captureRegion(Rectangle region)
        {
            Bitmap bmp = new Bitmap(region.Width, region.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(region.Location, Point.Empty , region.Size);
            }

            return bmp;
        }

        public static Bitmap toGrayScale(Bitmap original)
        {
            Bitmap grayScale = new Bitmap(original.Width, original.Height);
            using (Graphics g = Graphics.FromImage(grayScale))
            {
                ColorMatrix colorMatrix = new ColorMatrix(
                    new float[][]
                    {
                        new float[] {0.299f, 0.299f, 0.299f, 0, 0},
                        new float[] {0.587f, 0.587f, 0.587f, 0, 0},
                        new float[] {0.114f, 0.114f, 0.114f, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {0, 0, 0, 0, 1}
                    });
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);
                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                    0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }
            return grayScale;
        }

		public static Bitmap Threshold(Bitmap src, byte threshold = 160)
		{
			Bitmap dst = new Bitmap(src.Width, src.Height);

			for (int y = 0; y < src.Height; y++)
			{
				for (int x = 0; x < src.Width; x++)
				{
					Color c = src.GetPixel(x, y);
					byte v = (byte)((c.R + c.G + c.B) / 3);
					dst.SetPixel(x, y, v > threshold ? Color.White : Color.Black);
				}
			}
			return dst;
		}
	}
}
