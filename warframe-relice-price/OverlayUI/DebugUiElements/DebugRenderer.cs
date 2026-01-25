using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using warframe_relice_price.OCRVision;
using warframe_relice_price.WarframeTracker;

namespace OverlayUI.DebugUiElements;

internal class DebugRenderer
{
    private Canvas _canvas;
    private double height;
    private double width;

    public DebugRenderer(Canvas debugCanvas)
    {
        _canvas = debugCanvas;
        height = debugCanvas.Height;
        width = debugCanvas.Width;
    }

    private static Rect PxToDip(System.Drawing.Rectangle px)
    {
        return new Rect(
            px.X / WarframeWindowInfo.DpiX,
            px.Y / WarframeWindowInfo.DpiY,
            px.Width / WarframeWindowInfo.DpiX,
            px.Height / WarframeWindowInfo.DpiY
        );
    }

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

        _canvas.Children.Add(rect);
    }


    public void DrawDebugRewardBoxes()
    {
        for (int i = 1; i <= 4; i++)
        {
            System.Drawing.Rectangle rectangle = ScreenCaptureRow.GetRewardBoxPx(i, 4);
            int left = rectangle.Left;
            DrawDebugRectPx(rectangle, Colors.LimeGreen);
        }
    }

    public void DrawDebugRectPx(System.Drawing.Rectangle pxRect, Color color)
    {
        var dip = PxToDip(pxRect);

        var r = new Rectangle
        {
            Width = dip.Width,
            Height = dip.Height,
            Stroke = new SolidColorBrush(color),
            StrokeThickness = 2,
            Fill = Brushes.Transparent
        };

        Canvas.SetLeft(r, dip.X);
        Canvas.SetTop(r, dip.Y);

        _canvas.Children.Add(r);
    }

    public void drawMiddleBox(Rect dip)
    {
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
        _canvas.Children.Add(r);
    }

    // Crosshair at (dip.X, dip.Y)
    public void drawCrosshard(Rect dip)
    {
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

        _canvas.Children.Add(crossV);
        _canvas.Children.Add(crossH);
    }

    public void drawDebugText(Rect dip)
    {
        var info = new TextBlock
        {
            Foreground = Brushes.White,
            FontSize = 14,
            Text =
        $"DpiX={WarframeWindowInfo.DpiX:F3} DpiY={WarframeWindowInfo.DpiY:F3}\n" +
        $"WindowPx={WarframeWindowInfo.WidthPx}x{WarframeWindowInfo.HeightPx}\n" +
        $"WindowDip={WarframeWindowInfo.WidthDip:F1}x{WarframeWindowInfo.HeightDip:F1}\n" +
        $"TestRectPx={dip} => RectDip(X={dip.X:F1},Y={dip.Y:F1},W={dip.Width:F1},H={dip.Height:F1})"
        };

        Canvas.SetLeft(info, 20);
        Canvas.SetTop(info, 20);
        _canvas.Children.Add(info);
    }

    public void drawSanityCheck()
    {
        const int rectWpx = 200;
        const int rectHpx = 100;

        int x = (WarframeWindowInfo.WidthPx - rectWpx) / 2;
        int y = (WarframeWindowInfo.HeightPx - rectHpx) / 2;

        // A known, fixed pixel rect (window-relative).
        var px = new System.Drawing.Rectangle(x, y, rectWpx, rectHpx);
        var dipRect = PxToDip(px);

        drawMiddleBox(dipRect);
        drawCrosshard(dipRect);
        drawDebugText(dipRect);

    }
}
