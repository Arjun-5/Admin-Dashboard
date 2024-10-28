using StressCommunicationAdminPanel.Helpers;
using StressCommunicationAdminPanel.SplashScreen;
using System.Windows;

namespace StressCommunicationAdminPanel
{
  public partial class App : Application
  {
    void AppStartup(object sender, StartupEventArgs e)
    {
      var appConfig = ConfigHandler.LoadConfig();

      if (!appConfig.shouldUseSplashScreen)
      {
        var startupWindow = new MainWindow();

        startupWindow.Show();
      }
      else
      {
        var splashScreenWindow = new StartupSplashScreen();

        splashScreenWindow.Show();
      }
    }
  }
}
