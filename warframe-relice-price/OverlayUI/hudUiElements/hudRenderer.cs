using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using warframe_relice_price.OverlayUI;

namespace OverlayUI.hudUiElements;
internal class hudRenderer
{
    private Canvas _canvas;
    private double height;
    private double width;
    private TextBlock _loadingTextBlock;

    public hudRenderer(Canvas hudCanvas) 
    { 
        _canvas = hudCanvas;
        height = hudCanvas.Height;
        width = hudCanvas.Width;
    }

    public void DrawRelicPrices(List<int?> prices)
    {
        _canvas.Children.Clear();

        double width = _canvas.ActualWidth;
        double height = _canvas.ActualHeight;

        if (width <= 0 || height <= 0) return;

        int slots = prices.Count;

        double rowY = height * OverlayConstants.RewardRowYPercent;
        double priceOffsetY = rowY - (height * OverlayConstants.PriceOffsetYPercent);

        double totalRowWidth = width * OverlayConstants.TotalRowWidthPercent;
        double startX = (width - totalRowWidth) / 2;
        double slotWidth = totalRowWidth / slots;

        for (int i = 0; i < slots; i++)
        {
            double slotX = startX + i * slotWidth;
            string displayText = prices[i] is null ? "No listings" : $"{prices[i]}p";

            var priceText = new TextBlock
            {
                Text = displayText,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Gold,
                FontSize = height * 0.018,
                TextAlignment = TextAlignment.Center
            };

            priceText.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            Canvas.SetLeft(priceText, slotX + (slotWidth - priceText.DesiredSize.Width) / 2);
            Canvas.SetTop(priceText, priceOffsetY);

            _canvas.Children.Add(priceText);

            // Remove after 1.5 seconds
            _ = RemoveAfterDelayAsync(priceText, 1500);
        }
    }

    private async Task RemoveAfterDelayAsync(UIElement element, int milliseconds)
    {
        await Task.Delay(milliseconds);
        _canvas.Dispatcher.Invoke(() => _canvas.Children.Remove(element));
    }

    public void DrawGuiActiveText()
    {
        double x = _canvas.ActualWidth / 2;

        var text = new TextBlock
        {
            Text = "RELIC OVERLAY ACTIVE",
            FontWeight = FontWeights.Bold,
            Foreground = System.Windows.Media.Brushes.White,
            FontSize = 14,
            TextAlignment = TextAlignment.Center
        };

        text.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));

        Canvas.SetLeft(text, x - text.DesiredSize.Width / 2);
        Canvas.SetTop(text, 20);

        var circle = new Ellipse
        {
            Width = 14,
            Height = 14,
            Fill = System.Windows.Media.Brushes.Green
        };

        Canvas.SetLeft(circle, x + text.DesiredSize.Width / 2 + 8);
        Canvas.SetTop(circle, 23);

        _canvas.Children.Add(text);
        _canvas.Children.Add(circle);
    }

    public void ShowLoadingIndicator()
    {
        double height = _canvas.ActualHeight;
        _loadingTextBlock = new TextBlock
        {
            Text = "Loading...",
            FontWeight = FontWeights.Bold,
            Foreground = Brushes.White,
            FontSize = 20,
            TextAlignment = TextAlignment.Center
        };
        _loadingTextBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        double x = (_canvas.ActualWidth - _loadingTextBlock.DesiredSize.Width) / 2;
        double y = height * OverlayConstants.RewardRowYPercent - (height * OverlayConstants.PriceOffsetYPercent);
        Canvas.SetLeft(_loadingTextBlock, x);
        Canvas.SetTop(_loadingTextBlock, y);

        _canvas.Children.Add(_loadingTextBlock);
    }

    public void HideLoadingIndicator()
    {
        if (_loadingTextBlock != null)
        {
            _canvas.Children.Remove(_loadingTextBlock);
            _loadingTextBlock = null;
        }
    }

    public void DrawItemsName(string text)
    {
        var itemNameText = new TextBlock
        {
            Text = text,
            FontWeight = FontWeights.Light,
            Foreground = System.Windows.Media.Brushes.White,
            FontSize = 16
        };
        itemNameText.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
        double textWidth = itemNameText.DesiredSize.Width;
    }

    public void DrawAll()
    {
        _canvas.Children.Clear();
        DrawGuiActiveText();
    }
}

