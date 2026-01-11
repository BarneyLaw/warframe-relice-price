using System.Windows;
using warframe_relice_price.Core;
using warframe_relice_price.Utils;

namespace warframe_relice_price;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private AppController? appController;

        public App()
        {
            DispatcherUnhandledException += (_, ex) =>
            {
                MessageBox.Show(ex.Exception.ToString(), "DispatcherUnhandledException");
                ex.Handled = true;
            };

            AppDomain.CurrentDomain.UnhandledException += (_, ex) =>
            {
                MessageBox.Show(ex.ExceptionObject?.ToString() ?? "Unknown exception", "UnhandledException");
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Logger.Log("Overlay starting up.");

            var window = new MainWindow();
            MainWindow = window;
            window.Show();
            Logger.Log("Main window shown.");

            appController = new AppController(window);
            appController.startLoop();           
        }
    }


