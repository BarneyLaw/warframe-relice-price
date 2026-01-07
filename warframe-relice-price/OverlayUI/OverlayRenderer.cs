
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            _overlayCanvas.Children.Clear();

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
                    Foreground = Brushes.Gold,
                    FontSize = height * 0.018 // Adjust font size relative to height
                };
                priceText.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
                double textWidth = priceText.DesiredSize.Width;

                Canvas.SetLeft(priceText, slotX + (slotWidth - textWidth) / 2);
                Canvas.SetTop(priceText, priceOffsetY);

                _overlayCanvas.Children.Add(priceText);
            }
        }
    }

    
    
}
