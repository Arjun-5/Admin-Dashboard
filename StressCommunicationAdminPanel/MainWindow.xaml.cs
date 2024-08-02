using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Timers;
using System;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;
using StressCommunicationAdminPanel.Model;
using StressCommunicationAdminPanel.ViewModel;

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
  /// View model for adding config value - text box to display the current Ip address, timer interval and port number
  /// view model for showing graph data for velocity and accleration and time stamp
  /// view model for showing graph data for self record stress value based on time stamp 
  /// view model for showing graph data for task start time and task end time
  /// status bar loader for sending stress message every seconds and the value of the stress should be between 0 (inclusive) and 1 (inclusive)
  /// 
  public partial class MainWindow : Window
  {
    private ObservableCollection<StressNotificationMessage> stressNotificationMessages { get; set; }

    public ObservableCollection<StressNotificationMessage>  StressNotificationMessages => stressNotificationMessages;

    public MainWindow()
    {
      InitializeComponent();

      DataContext = new StressMessageViewModel();
    }
    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      await Task.Run(() =>
      {
        SendBroadcastMessage();
      });
    }
    private void Border_MouseDownEvent(object sender, MouseButtonEventArgs e)
    {
      if(e.ChangedButton == MouseButton.Left)
      {
        this.DragMove();
      }
    }
    private void MainWindow_Close(object sender, RoutedEventArgs e)
    {
      App.Current.Shutdown();
    }
    private void SendBroadcastMessage()
    {
      UdpClient client = new UdpClient();

      IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 11000);

      byte[] message = Encoding.ASCII.GetBytes("Server IP Address Broadcast Message");

      client.Send(message, message.Length, endPoint);

      client.Close();

      SetupConnectionParameters(ProcessAndGetConfigInformation());
    }
    private StressMessageConfig ProcessAndGetConfigInformation()
    {
      string currentWorkingDirectory = Environment.CurrentDirectory;

      string configFileLocation = Path.Combine(currentWorkingDirectory, "StressMessageConfig.json");

      var processedConfigData = File.ReadAllText(configFileLocation);

      Console.WriteLine("The processed config data is : " + processedConfigData);

      return JsonConvert.DeserializeObject<StressMessageConfig>(processedConfigData);
    }
    private void SetupConnectionParameters(StressMessageConfig stressMessageConfig)
    {
      if (stressMessageConfig == null)
      {
        return;
      }

      var stressMessageTimer = new Timer(stressMessageConfig.messageTimeInterval);

      stressMessageTimer.Enabled = true;

      bool executeAtStart = false;

      Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

      serverSocket.Bind(new IPEndPoint(IPAddress.Parse(stressMessageConfig.ipAddress), stressMessageConfig.stressMessageSendingPort));

      serverSocket.Listen(1);

      Socket clientSocket = serverSocket.Accept();

      stressMessageTimer.Elapsed += (sender, e) => OnStressMessageTimerElapsed(sender, e, clientSocket, stressMessageConfig.messageTimeInterval);

      while (true)
      {
        if (!executeAtStart)
        {
          executeAtStart = true;

          SendStressMessage(clientSocket, stressMessageConfig.messageTimeInterval);
        }
        GetClientMessage(clientSocket);
      }
    }
    private void OnStressMessageTimerElapsed(object sender, ElapsedEventArgs e, Socket clientSocket, int seconds)
    {
      SendStressMessage(clientSocket, seconds);
    }
    private void SendStressMessage(Socket clientSocket, int seconds)
    {
      if (clientSocket == null)
      {
        return;
      }

      StressNotificationMessage stressNotificationMessage = new StressNotificationMessage(
        currentStressEffect: (StressEffectType)new Random().Next(0, 4),
        stressLevel: new Random().Next(0, 1));

      string serializedNotificationMessage = JsonConvert.SerializeObject(stressNotificationMessage, new StringEnumConverter());

      byte[] stressInfoMessage = Encoding.ASCII.GetBytes(serializedNotificationMessage);

      Trace.WriteLine("text"+ stressInfoMessage);

      clientSocket.Send(stressInfoMessage);

    }
    private void GetClientMessage(Socket clientSocket)
    {
      byte[] bytes = new byte[1024];

      int clientBytes = clientSocket.Receive(bytes);

      string clientMessage = Encoding.ASCII.GetString(bytes, 0, clientBytes);
    }
  }
}
