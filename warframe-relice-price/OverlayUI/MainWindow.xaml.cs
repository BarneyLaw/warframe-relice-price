using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using warframe_relice_price.Utils;

namespace warframe_relice_price
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //private void MakeClickThrough(object sender, RoutedEventArgs e)
        //{
        //    nint hwnd = new WindowInteropHelper(this).Handle;

        //    int exStyle = Win32.GetWindowLong(hwnd, Win32.GWL_EXSTYLE);
        //    nint newStyle = (nint)(exStyle | Win32.WS_EX_LAYERED | Win32.WS_EX_TRANSPARENT);

        //    Win32.SetWindowLong(hwnd, Win32.GWL_EXSTYLE, newStyle);

        //    Loaded -= MakeClickThrough;
        //}
    }
}