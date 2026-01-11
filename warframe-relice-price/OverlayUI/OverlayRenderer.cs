using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using Rewards.Processing;
using Rewards.Services;

using warframe_relice_price.OCRVision;

namespace warframe_relice_price.OverlayUI
{
	class OverlayRenderer
	{
		private readonly Canvas _overlayCanvas;
		private readonly WarframeMarketClient _marketClient;

		public OverlayRenderer(Canvas overlayCanvas)
		{
			_overlayCanvas = overlayCanvas;
			_marketClient = new WarframeMarketClient();
		}

		public async Task DrawRelicPricesAsync(double width, double height, int slots)
		{
			if (width <= 0 || height <= 0 || slots <= 0)
				return;

			double rowY = height * OverlayConstants.RewardRowYPercent;
			double priceOffsetY = rowY - (height * OverlayConstants.PriceOffsetYPercent);

			double totalRowWidth = width * OverlayConstants.TotalRowWidthPercent;
			double startX = (width - totalRowWidth) / 2;
			double slotWidth = totalRowWidth / slots;

			var screenRowRect = ScreenCaptureRow.ToScreenRect(ScreenCaptureRow.row_rect);
			using var bmp = ScreenCaptureRow.captureRegion(screenRowRect);

			string ocrText = ImageToText.multiPassOCR(bmp);
			var items = RewardCounter.Count(ocrText);

			for (int i = 0; i < slots; i++)
			{
				double slotX = startX + i * slotWidth;
				string displayText = "—";

				if (i < items.Count)
				{
					string urlName = WarframeMarketNaming.ToUrlName(items[i].CanonicalName);
					Console.WriteLine(urlName);
					int? price = await _marketClient.GetLowestPriceAsync(urlName);
					displayText = price is null ? "No listings" : $"{price}p";
				}

				var priceText = new TextBlock
				{
					Text = displayText,
					FontWeight = FontWeights.Bold,
					Foreground = Brushes.Gold,
					FontSize = height * 0.018,
					TextAlignment = TextAlignment.Center
				};

				priceText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

				Canvas.SetLeft(priceText, slotX + (slotWidth - priceText.DesiredSize.Width) / 2);
				Canvas.SetTop(priceText, priceOffsetY);

				_overlayCanvas.Children.Add(priceText);

				// --- Remove after 15 seconds ---
				_ = RemoveAfterDelayAsync(priceText, 15000);
			}
		}

		// Helper to remove a TextBlock after a delay
		private async Task RemoveAfterDelayAsync(UIElement element, int milliseconds)
		{
			await Task.Delay(milliseconds);

			// Remove safely on UI thread
			_overlayCanvas.Dispatcher.Invoke(() =>
			{
				_overlayCanvas.Children.Remove(element);
			});
		}


		// Debug / boundary visualization
		public void DrawTestBoundary()
		{
			var detectionRect = new Rectangle
			{
				Width = ScreenCaptureRow.detection_box_width,
				Height = ScreenCaptureRow.detection_box_height,
				Stroke = Brushes.Red,
				StrokeThickness = 2,
				Fill = Brushes.Transparent
			};

			Canvas.SetLeft(detectionRect, ScreenCaptureRow.detection_box_x_coordinate);
			Canvas.SetTop(detectionRect, ScreenCaptureRow.detection_box_y_coordinate);

			_overlayCanvas.Children.Add(detectionRect);
		}

		public async Task DrawAllAsync(double width, double height, int slots)
		{
			_overlayCanvas.Children.Clear();

			await DrawRelicPricesAsync(width, height, slots);
			DrawTestBoundary();
		}
	}
}
