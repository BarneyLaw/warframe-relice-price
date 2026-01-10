using System;
using System.Drawing;
using System.Drawing.Imaging;

using warframe_relice_price.WarframeTracker;

namespace warframe_relice_price.OCRVision
{
    public static class ScreenCaptureRow
    {

        // Dimensions for the Detection Box that determines Reward Screen Status
        public static int detection_box_x_coordinate = (int) (0.0729 * WarframeWindowInfo.Width);
        public static int detection_box_y_coordinate = (int) (0.0324 * WarframeWindowInfo.Height);
        public static int detection_box_width = (int) (0.401 * WarframeWindowInfo.Width);
        public static int detection_box_height = (int) (0.0602 * WarframeWindowInfo.Height);

        public static Rectangle detection_box_rect = new Rectangle(
            detection_box_x_coordinate,
            detection_box_y_coordinate,
            detection_box_width,
            detection_box_height
        );

        // Dimensions for the whole Reward row
        public static int row_x_coordinate = (int) (0.20 * WarframeWindowInfo.Width);
        public static int row_y_coordinate = (int) (0.379 * WarframeWindowInfo.Height);
        public static int row_width = (int) (0.55 * WarframeWindowInfo.Width);
        public static int row_height = (int) (0.056 * WarframeWindowInfo.Height);

        public static Rectangle row_rect = new Rectangle(
            row_x_coordinate,
            row_y_coordinate,
            row_width,
            row_height
        );

        // Dimensions for invidual boxes (n = 1 to 4)
        public static Rectangle get_box_rect(int n)
        {
            int box_width = (int) (0.13 * WarframeWindowInfo.Width);
            int box_height = (int) (0.056 * WarframeWindowInfo.Height);
            int box_x_coordinate = row_x_coordinate + (n - 1) * box_width;
            int box_y_coordinate = row_y_coordinate;
            return new Rectangle(
                box_x_coordinate,
                box_y_coordinate,
                box_width,
                box_height
            );
        }

        public static Rectangle ToScreenRect(Rectangle overlayRect)
        {
            return new Rectangle(
                WarframeWindowInfo.XOffset + overlayRect.X,
                WarframeWindowInfo.YOffset + overlayRect.Y,
                overlayRect.Width,
                overlayRect.Height
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
