using System;
using System.Windows;

namespace StressCommunicationAdminPanel.SplashScreen
{
  public partial class StartupSplashScreen : Window
  {
      public StartupSplashScreen()
      {
          InitializeComponent();
      }
    private void SplashScreenStoryboard_Completed(object sender, EventArgs e)
    {
      var newWindow = new MainWindow(); 

      newWindow.Show();

      this.Close();
    }
  }
}