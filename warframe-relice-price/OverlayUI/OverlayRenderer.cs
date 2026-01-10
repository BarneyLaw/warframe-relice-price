
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using warframe_relice_price.OCRVision;

namespace warframe_relice_price.OverlayUI
{
    class OverlayRenderer
    {
        private readonly Canvas _overlayCanvas;

        public OverlayRenderer(Canvas overlayCanvas)
        {
            _overlayCanvas = overlayCanvas;
        }

        // Replace this later with actual rendering logic
        public void DrawFakeRelicPrices(double width, double height, int slots)
        {
            //_overlayCanvas.Children.Clear();

            if (width <= 0 || height <= 0) return;

            double rowY = height * OverlayConstants.RewardRowYPercent;
            double priceOffsetY = rowY - (height * OverlayConstants.PriceOffsetYPercent);

            double totalRowWidth = width * OverlayConstants.TotalRowWidthPercent; 
            double startX = (width - totalRowWidth) / 2;
            double slotWidth = totalRowWidth / slots;

            for (int i = 0; i < slots; i++)
            {
                double slotX = startX + (i * slotWidth);
                // Draw fake price text
                var priceText = new TextBlock
                {
                    Text = $"Price: {(i + 1) * 10} Platinum",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Gold,
                    FontSize = height * 0.018 // Adjust font size relative to height
                };
                priceText.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
                double textWidth = priceText.DesiredSize.Width;

                Canvas.SetLeft(priceText, slotX + (slotWidth - textWidth) / 2);
                Canvas.SetTop(priceText, priceOffsetY);

                _overlayCanvas.Children.Add(priceText);
            }
        }

        // To Test if boundary is correctly set
        public void DrawTestBoundary()
        {
            //_overlayCanvas.Children.Clear();

            // Reward Screen Detection Box
            var detectionRect = new System.Windows.Shapes.Rectangle
            {
                Width = ScreenCaptureRow.detection_box_width,
                Height = ScreenCaptureRow.detection_box_height,
                Stroke = System.Windows.Media.Brushes.Red,
                StrokeThickness = 2,
                Fill = System.Windows.Media.Brushes.Transparent
            };

            Canvas.SetLeft(detectionRect, ScreenCaptureRow.detection_box_x_coordinate);
            Canvas.SetTop(detectionRect, ScreenCaptureRow.detection_box_y_coordinate);
            _overlayCanvas.Children.Add(detectionRect);

            // Reward Row Box
            var rowRect = new System.Windows.Shapes.Rectangle
            {
                Width = ScreenCaptureRow.row_width,
                Height = ScreenCaptureRow.row_height,
                Stroke = System.Windows.Media.Brushes.Blue,
                StrokeThickness = 2,
                Fill = System.Windows.Media.Brushes.Transparent
            };

            Canvas.SetLeft(rowRect, ScreenCaptureRow.row_x_coordinate);
            Canvas.SetTop(rowRect, ScreenCaptureRow.row_y_coordinate);
            _overlayCanvas.Children.Add(rowRect);
        }

        public void DrawAll(double width, double height, int slots)
        {
            _overlayCanvas.Children.Clear();

            DrawFakeRelicPrices(width, height, slots);
            DrawTestBoundary();
        }
    }



}
