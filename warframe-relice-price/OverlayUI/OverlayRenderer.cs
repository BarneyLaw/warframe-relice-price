using Rewards.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using warframe_relice_price.OCRVision;
using warframe_relice_price.WarframeTracker;

namespace warframe_relice_price.OverlayUI
{
	class OverlayRenderer
	{
        private readonly Canvas _hudCanvas;
        private readonly Canvas _menuCanvas; 
        private readonly WarframeMarketClient _marketClient = new();
        private TextBlock _loadingTextBlock;

        private Rectangle? _dimLayer;
        private Border? _menuPanel;

        public bool IsOverlayMenuOpen { get; private set; }

        public OverlayRenderer(Canvas hudCanvas, Canvas menuCanvas)
		{
            _hudCanvas = hudCanvas;
            _menuCanvas = menuCanvas;
        }

        public void OpenOverlayMenu()
        {
            IsOverlayMenuOpen = true;
            EnsureMenuVisuals();
            SetMenuVisibility(visible: true);
        }

        public void CloseOverlayMenuIfOpen()
        {
            if (!IsOverlayMenuOpen)
                return;

            IsOverlayMenuOpen = false;
            SetMenuVisibility(visible: false);
        }

        public void UpdateOverlayMenuLayout()
        {
            if (_dimLayer == null || _menuPanel == null)
                return;

            double w = _menuCanvas.ActualWidth;
            double h = _menuCanvas.ActualHeight;
            if (w <= 0 || h <= 0)
                return;

            _dimLayer.Width = w;
            _dimLayer.Height = h;

            _menuPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double mw = _menuPanel.DesiredSize.Width;
            double mh = _menuPanel.DesiredSize.Height;

            Canvas.SetLeft(_menuPanel, (w - mw) / 2);
            Canvas.SetTop(_menuPanel, (h - mh) / 2);
        }

        private void EnsureMenuVisuals()
        {
            if (_dimLayer == null)
            {
                _dimLayer = new Rectangle
                {
                    Fill = new SolidColorBrush(Color.FromArgb(140, 0, 0, 0)),
                    Visibility = Visibility.Collapsed,
                    IsHitTestVisible = true
                };
                _menuCanvas.Children.Add(_dimLayer);
            }

            if (_menuPanel == null)
            {
                var stack = new StackPanel();
                stack.Children.Add(new TextBlock
                {
                    Text = "Relic Overlay",
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 0, 0, 8)
                });
                stack.Children.Add(new TextBlock
                {
                    Text = "X / Esc: close menu\nQ: quit app\nShift+F9: toggle",
                    FontSize = 14,
                    Foreground = Brushes.Gainsboro
                });

                _menuPanel = new Border
                {
                    Background = new SolidColorBrush(Color.FromArgb(230, 20, 20, 20)),
                    BorderBrush = Brushes.White,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(16),
                    Child = stack,
                    Visibility = Visibility.Collapsed,
                    IsHitTestVisible = true
                };

                _menuCanvas.Children.Add(_menuPanel);
            }
        }

        private void SetMenuVisibility(bool visible)
        {
            if (_dimLayer != null) _dimLayer.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            if (_menuPanel != null) _menuPanel.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public void DrawRelicPrices(List<int?> prices)
		{
			_hudCanvas.Children.Clear();

			double width = _hudCanvas.ActualWidth;
			double height = _hudCanvas.ActualHeight;

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

                _hudCanvas.Children.Add(priceText);

				// Remove after 1.5 seconds
				_ = RemoveAfterDelayAsync(priceText, 1500);
			}
		}

		private async Task RemoveAfterDelayAsync(UIElement element, int milliseconds)
		{
			await Task.Delay(milliseconds);
            _hudCanvas.Dispatcher.Invoke(() => _hudCanvas.Children.Remove(element));
		}

		private Rect PxToDip(System.Drawing.Rectangle px)
        {
            return new Rect(
                px.X / WarframeWindowInfo.DpiX,
                px.Y / WarframeWindowInfo.DpiY,
                px.Width / WarframeWindowInfo.DpiX,
                px.Height / WarframeWindowInfo.DpiY
            );
        }


        // Debug / boundary visualization
        public void DrawTestBoundary()
        {
            var pxRect = ScreenCaptureRow.GetDetectionBoxPx();
            var dipRect = PxToDip(pxRect);

            var rect = new System.Windows.Shapes.Rectangle
            {
                Width = dipRect.Width,
                Height = dipRect.Height,
                Stroke = System.Windows.Media.Brushes.Red,
                StrokeThickness = 2,
                Fill = System.Windows.Media.Brushes.Transparent
            };

            Canvas.SetLeft(rect, dipRect.X);
            Canvas.SetTop(rect, dipRect.Y);

            _hudCanvas.Children.Add(rect);
        }

        
        public void DrawDebugRewardBoxes()
        {
            for (int i = 1; i <= 4; i++)
            {
                System.Drawing.Rectangle rectangle = ScreenCaptureRow.GetRewardBoxPx(i, 4);
                int left = rectangle.Left;
                DrawDebugRectPx(rectangle, System.Windows.Media.Colors.LimeGreen);
            }
        }

        public void DrawDebugRectPx(System.Drawing.Rectangle pxRect, System.Windows.Media.Color color)
        {
            var dip = PxToDip(pxRect);

            var r = new System.Windows.Shapes.Rectangle
            {
                Width = dip.Width,
                Height = dip.Height,
                Stroke = new SolidColorBrush(color),
                StrokeThickness = 2,
                Fill = System.Windows.Media.Brushes.Transparent
            };

            Canvas.SetLeft(r, dip.X);
            Canvas.SetTop(r, dip.Y);

            _hudCanvas.Children.Add(r);
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

        public void DrawDpiSanityTest()
        {
            // If these are 0/NaN, your window-info update isn't running before rendering.
            if (WarframeWindowInfo.DpiX <= 0 || WarframeWindowInfo.DpiY <= 0)
                return;

            const int rectWpx = 200;
            const int rectHpx = 100;

            int x = (WarframeWindowInfo.WidthPx - rectWpx) / 2;
            int y = (WarframeWindowInfo.HeightPx - rectHpx) / 2;

            // A known, fixed pixel rect (window-relative).
            var px = new System.Drawing.Rectangle(x, y, rectWpx, rectHpx);
            var dip = PxToDip(px);

            var r = new System.Windows.Shapes.Rectangle
            {
                Width = dip.Width,
                Height = dip.Height,
                Stroke = Brushes.Lime,
                StrokeThickness = 2,
                Fill = Brushes.Transparent
            };

            Canvas.SetLeft(r, dip.X);
            Canvas.SetTop(r, dip.Y);
            _hudCanvas.Children.Add(r);

            // Crosshair at (dip.X, dip.Y)
            var crossV = new System.Windows.Shapes.Line
            {
                X1 = dip.X + dip.Width / 2,
                Y1 = dip.Y + dip.Height / 2 - 10,
                X2 = dip.X + dip.Width / 2,
                Y2 = dip.Y + dip.Height / 2 + 10,
                Stroke = Brushes.Lime,
                StrokeThickness = 2
            };
            var crossH = new System.Windows.Shapes.Line
            {
                X1 = dip.X + dip.Width / 2 - 10,
                Y1 = dip.Y + dip.Height / 2,
                X2 = dip.X + dip.Width / 2 + 10,
                Y2 = dip.Y + dip.Height / 2,
                Stroke = Brushes.Lime,
                StrokeThickness = 2
            };

            _hudCanvas.Children.Add(crossV);
            _hudCanvas.Children.Add(crossH);

            var info = new TextBlock
            {
                Foreground = Brushes.White,
                FontSize = 14,
                Text =
                    $"DpiX={WarframeWindowInfo.DpiX:F3} DpiY={WarframeWindowInfo.DpiY:F3}\n" +
                    $"WindowPx={WarframeWindowInfo.WidthPx}x{WarframeWindowInfo.HeightPx}\n" +
                    $"WindowDip={WarframeWindowInfo.WidthDip:F1}x{WarframeWindowInfo.HeightDip:F1}\n" +
                    $"TestRectPx={px} => RectDip(X={dip.X:F1},Y={dip.Y:F1},W={dip.Width:F1},H={dip.Height:F1})"
            };

            Canvas.SetLeft(info, 20);
            Canvas.SetTop(info, 20);
            _hudCanvas.Children.Add(info);
        }

        public void DrawGuiActiveText()
        {
            double x = _hudCanvas.ActualWidth / 2;

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

            _hudCanvas.Children.Add(text);
            _hudCanvas.Children.Add(circle);
        }

        public void ShowLoadingIndicator()
        {
            double height = _hudCanvas.ActualHeight;
            _loadingTextBlock = new TextBlock
            {
                Text = "Loading...",
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                FontSize = 20,
                TextAlignment = TextAlignment.Center
            };
            _loadingTextBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double x = (_hudCanvas.ActualWidth - _loadingTextBlock.DesiredSize.Width) / 2;
            double y = height * OverlayConstants.RewardRowYPercent - (height * OverlayConstants.PriceOffsetYPercent);
            Canvas.SetLeft(_loadingTextBlock, x);
            Canvas.SetTop(_loadingTextBlock, y);

            _hudCanvas.Children.Add(_loadingTextBlock);
        }

        public void HideLoadingIndicator()
        {
            if (_loadingTextBlock != null)
            {
                _hudCanvas.Children.Remove(_loadingTextBlock);
                _loadingTextBlock = null;
            }
        }

        public void DrawAll()
        {
            _hudCanvas.Children.Clear();
            DrawGuiActiveText();
        }
    }
}
