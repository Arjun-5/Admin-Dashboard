using FontAwesome.Sharp;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StressCommunicationAdminPanel.Command;
using StressCommunicationAdminPanel.Model;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using LiveChartsCore.SkiaSharpView.Painting;

namespace StressCommunicationAdminPanel.ViewModel
{
  public class StressMessageViewModel : AppViewModel
  {
    private ObservableCollection<StressMessage> _messages = new ObservableCollection<StressMessage>();

    private ObservableCollection<PieSeries<int>> _stressEffectMessagesSeriesCollection;

    private UdpClient _client;
    
    private Timer _stressMessageTimer;

    private Socket _clientSocket;

    private string _connectionStatus;

    private IconChar _connectionStatusIcon;

    private bool _serverRunning = false;

    private int _messagesSent = 0;

    private bool _hasStressEffectMessage = false;
    public ObservableCollection<StressMessage> Messages
    {
      get { return _messages; }

      set { _messages = value; OnPropertyChanged(nameof(Messages)); }
    }

    public ObservableCollection<PieSeries<int>> StressEffectMessagesSeriesCollection
    {
      get { return _stressEffectMessagesSeriesCollection; }

      set { _stressEffectMessagesSeriesCollection = value; OnPropertyChanged(nameof(StressEffectMessagesSeriesCollection)); }
    }
    public string ConnectionStatus
    {
      get { return _connectionStatus; }
      
      set { _connectionStatus = value; OnPropertyChanged(nameof(ConnectionStatus)); }
    }
    public IconChar ConnectionStatusIcon
    {
      get { return _connectionStatusIcon; }
     
      set { _connectionStatusIcon = value; OnPropertyChanged(nameof(ConnectionStatusIcon)); }
    }
    public int MessagesSent
    {
      get { return _messagesSent; }

      set { _messagesSent = value; OnPropertyChanged(nameof(MessagesSent)); }
    }
    public ICommand ToggleServerStateCommand { get; }

    public StressMessageViewModel()
    {
      Console.WriteLine("StressMessageViewModel Window Loaded");

      ConfigureStressMessagePieChartAttributes();

       ToggleServerStateCommand = new RelayCommand(ManageServerState);

      ConnectionStatus = "Not Connected!!!";

      ConnectionStatusIcon = IconChar.UserSlash;
    }
    private void ConfigureStressMessagePieChartAttributes()
    {
      _stressEffectMessagesSeriesCollection = new ObservableCollection<PieSeries<int>>
      {
        new PieSeries<int>
        {
          Name = "No Valid data",
          Values = new ObservableCollection<int> { 1 },
          Fill =new SolidColorPaint(SKColor.Parse("#1a1b26"))
        }
      };
    }
    private void ManageServerState()
    {
      if (_serverRunning)
      {
        StopServer();
      }
      else
      {
        StartServer();
      }
    }
    private async void StartServer()
    {
      _serverRunning = true;

      ConnectionStatus = "Starting...";
      
      ConnectionStatusIcon = IconChar.UserClock;

      SendBroadcastMessage();

      var config = ProcessAndGetConfigInformation();

      if (config != null)
      {
        await SetupConnectionParameters(config);
      }
      else
      {
        ConnectionStatus = "Failed to Load Config";

        ConnectionStatusIcon = IconChar.ExclamationTriangle;
      }
    }
    private void StopServer()
    {
      _serverRunning = false;
      
      _clientSocket?.Shutdown(SocketShutdown.Both);
      
      _clientSocket?.Close();
      
      _client?.Close();

      ConnectionStatus = "Stopped";

      _stressMessageTimer.Stop();

      ConnectionStatusIcon = IconChar.UserTimes;
    }
    private void SendBroadcastMessage()
    {
      _client = new UdpClient();

      IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 11000);
      
      byte[] message = Encoding.ASCII.GetBytes("Server IP Address Broadcast Message");
      
      _client.Send(message, message.Length, endPoint);

      Console.WriteLine("Broadcast Message Sent...");

      _client.Close();
    }
    private StressMessageConfig ProcessAndGetConfigInformation()
    {
      string currentWorkingDirectory = Environment.CurrentDirectory;
      
      string configFileLocation = Path.Combine(currentWorkingDirectory, "StressMessageConfig.json");
      
      try
      {
        var processedConfigData = File.ReadAllText(configFileLocation);

        return JsonConvert.DeserializeObject<StressMessageConfig>(processedConfigData);

      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error loading config: {ex.Message}");

        return null;
      }
    }

    private async Task SetupConnectionParameters(StressMessageConfig config)
    {
      if (config == null)
      {
        Console.WriteLine("Please check the data in your config file");

        return;
      }

      try
      {
        _stressMessageTimer = new Timer(config.messageTimeInterval);

        _stressMessageTimer.Elapsed += (sender, e) => OnStressMessageTimerElapsed(sender, e, config);

        var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        serverSocket.Bind(new IPEndPoint(IPAddress.Parse(config.ipAddress), config.stressMessageSendingPort));

        serverSocket.Listen(2);

        Console.WriteLine("Server is listening for connections");

        _clientSocket = await Task.Run(() =>
        {
          try
          {
            return serverSocket.Accept();
          }
          catch (SocketException ex)
          {
            Console.WriteLine($"SocketException: {ex.Message}");

            return null;
          }
          catch (Exception ex)
          {
            Console.WriteLine($"Exception: {ex.Message}");

            return null;
          }
        });
        
        if (_clientSocket == null)
        {
          Console.WriteLine("Failed to accept a client connection.");

          return;
        }

        ConnectionStatus = "Connected";

        ConnectionStatusIcon = IconChar.UserCheck;

        Console.WriteLine("Client connected!");

        SendStressMessage(config);

        _stressMessageTimer.Start();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Exception: {ex.Message}");
        
        ConnectionStatus = "Error";
       
        ConnectionStatusIcon = IconChar.ExclamationTriangle;
      }
    }
    private void OnStressMessageTimerElapsed(object sender, ElapsedEventArgs e, StressMessageConfig config)
    {
      SendStressMessage(config);
    }
    private void SendStressMessage(StressMessageConfig config)
    {
      if (_clientSocket == null)
      {
        return;
      }
      try
      {
        var stressNotificationMessage = new StressNotificationMessage(
         (StressEffectType)Random.Shared.Next(0, 3),
         Random.Shared.Next(0, 2));

        string serializedMessage = JsonConvert.SerializeObject(stressNotificationMessage, new StringEnumConverter());

        byte[] messageBytes = Encoding.ASCII.GetBytes(serializedMessage);

        Console.WriteLine("Sending stress notification: " + serializedMessage);

        MessagesSent++;

        _clientSocket.Send(messageBytes);

        Messages.Add(new StressMessage
        {
          timeSent = DateTime.Now,
          message = serializedMessage
        });

        UpdateStressMessageSentPieChartData(stressNotificationMessage.currentStressEffect);
      }
      catch (SocketException ex)
      {
        Console.WriteLine($"SocketException: {ex.Message}");
      }
    }
    private void UpdateStressMessageSentPieChartData(StressEffectType effectType)
    {
      if (!_hasStressEffectMessage)
      {
        StressEffectMessagesSeriesCollection.Clear();

        foreach (StressEffectType type in Enum.GetValues(typeof(StressEffectType)))
        {
          StressEffectMessagesSeriesCollection.Add(new PieSeries<int>
          {
            Name = Enum.GetName(typeof(StressEffectType), type),
            Values = new ObservableCollection<int> { 0 },
            Fill = GetColorForStressType(type),
            DataLabelsPaint = new SolidColorPaint(SKColors.Black),
            DataLabelsSize = 22,
            DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
          });
        }

        _hasStressEffectMessage = true;
      }

      string effectTypeName = Enum.GetName(typeof(StressEffectType), effectType);

      var series = StressEffectMessagesSeriesCollection.FirstOrDefault(s => s.Name == effectTypeName);

      if (series != null)
      {
        var valuesCollection = series.Values as ObservableCollection<int>;

        if (valuesCollection != null)
        {
          valuesCollection[0]++;
        }
        else
        {
          series.Values = new ObservableCollection<int> { series.Values.First() + 1 };
        }
      }
    }
    private SolidColorPaint GetColorForStressType(StressEffectType type)
    {
      switch (type)
      {
        case StressEffectType.Mental:
          return new SolidColorPaint(SKColor.Parse("#f7768e"));
        case StressEffectType.Emotional:
          return new SolidColorPaint(SKColor.Parse("#9ece6a"));
        case StressEffectType.Physical:
          return new SolidColorPaint(SKColor.Parse("#2ac3de"));
        default:
          return new SolidColorPaint(SKColor.Parse("#1a1b26"));
      }
    }
  }
}


