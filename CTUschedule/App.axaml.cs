using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CTUschedule.ViewModels;
using CTUschedule.Views;
using Utilities;

namespace CTUschedule
{
    public partial class App : Application
    {

        public static string _version = "0.5.2 Beta";
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Exit += Desktop_Exit;
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);

                CTU_HTQLWebDriver HTQLWebDriver = new CTU_HTQLWebDriver();
                //desktop.MainWindow = new NotificationPopup();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };


            }

            base.OnFrameworkInitializationCompleted();
        }

        private void Desktop_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            CTU_HTQLWebDriver.Instance.CloseWeb();
        }
    }
}