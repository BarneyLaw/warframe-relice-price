using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace OverlayUI.MenuUiElements;

internal class MenuRenderer
{
    private Canvas _canvas;

    private Rectangle? _dimLayer;
    private Border? _menuPanel;
    public bool IsOverlayMenuOpen { get; private set; }
    public MenuRenderer(Canvas menuCanvas)
    {
        _canvas = menuCanvas;
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

        double w = _canvas.ActualWidth;
        double h = _canvas.ActualHeight;
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
            _canvas.Children.Add(_dimLayer);
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

            _canvas.Children.Add(_menuPanel);
        }
    }

    private void SetMenuVisibility(bool visible)
    {
        if (_dimLayer != null) _dimLayer.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        if (_menuPanel != null) _menuPanel.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
    }
}
