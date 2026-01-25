using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using OverlayUI.MenuUiElements;
using OverlayUI.DebugUiElements;
using OverlayUI.hudUiElements;

namespace warframe_relice_price.OverlayUI
{
	class OverlayRenderer
	{
        public MenuRenderer Menu { get; }
        public hudRenderer Hud { get; }
        public DebugRenderer Debug { get; }

        public bool IsOverlayMenuOpen => Menu.IsOverlayMenuOpen;

        public OverlayRenderer(Canvas hudCanvas, Canvas menuCanvas)
		{
            Menu = new MenuRenderer(menuCanvas);
            Hud = new hudRenderer(hudCanvas);
            Debug = new DebugRenderer(hudCanvas);
        }
    }
}
