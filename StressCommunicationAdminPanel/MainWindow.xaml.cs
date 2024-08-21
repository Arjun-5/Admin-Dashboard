using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Timers;
using System;
using System.Windows;
using System.Windows.Input;
using StressCommunicationAdminPanel.ViewModels;
using StressCommunicationAdminPanel.Panel_User_Controls;

namespace StressCommunicationAdminPanel
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  /// 
  /// To Do List 
  /// 
  /// Add a start/loading screen
  /// Send task message containing tasks
  /// Send stress effects
  /// Add view model for different views
  /// view model for showing graph data for velocity and accleration and time stamp
  /// view model for showing graph data for self record stress value based on time stamp 
  /// view model for showing graph data for task start time and task end time
  /// status bar loader for sending stress message every seconds and the value of the stress should be between 0 (inclusive) and 1 (inclusive)
  /// 
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      DataContext = new StressCommunicationAppMainViewModel(messageProgressBar);
    }
    private void Border_MouseDownEvent(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
      {
        this.DragMove();
      }
    }
    private void MainWindow_Close(object sender, RoutedEventArgs e)
    {
      App.Current.Shutdown();
    }
  }
}