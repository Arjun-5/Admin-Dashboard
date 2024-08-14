using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using FontAwesome.Sharp;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using StressCommunicationAdminPanel.Models;
using System.Windows.Media;
using System.IO;

namespace StressCommunicationAdminPanel.Services
{
  public class StressMessageManager : PropertyChangeHandler
  {
    private UdpClient _client;
    
    private Socket _clientSocket;
    
    private Timer _stressMessageTimer;
    
    private Action<ServerState, IconChar, Brush, Brush> _onServerStateChanged;

    private Action<StressNotificationMessage> _onUpdateChartContent;

    private Action<IconChar, bool> _onHandleStatusBarState;

    private bool _serverRunning;

    private int _messagesSent;
    public int MessagesSent
    {
      get => _messagesSent;

      set
      {
        if (_messagesSent != value)
        {
          _messagesSent = value;

          OnPropertyChanged(nameof(MessagesSent));
        }
      }
    }
    public StressMessageManager(Action<ServerState, IconChar, Brush, Brush> onServerStateChanged, Action<StressNotificationMessage> onUpdateChartContent, Action<IconChar, bool> onHandleStatusBarState)
    {
      _onServerStateChanged = onServerStateChanged;

      _onUpdateChartContent = onUpdateChartContent;

      _onHandleStatusBarState = onHandleStatusBarState;
    }

    public void ManageServerState()
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

      UpdateServerState(ServerState.Starting, IconChar.UserClock, Brushes.OrangeRed, Brushes.OrangeRed);

      SendBroadcastMessage();
      
      var config = LoadConfig();

      if (config != null)
      {
        await SetupConnectionParameters(config);
      }
      else
      {
        UpdateServerState(ServerState.FailedToLoadConfig, IconChar.ExclamationTriangle, Brushes.Red, Brushes.OrangeRed);
      }
    }

    private void StopServer()
    {
      _serverRunning = false;
     
      _clientSocket?.Shutdown(SocketShutdown.Both);
      
      _clientSocket?.Close();
      
      _client?.Close();
      
      _stressMessageTimer?.Stop();
      
      UpdateServerState(ServerState.Stopped, IconChar.UserTimes, Brushes.Red, Brushes.OrangeRed);

      _onHandleStatusBarState?.Invoke(IconChar.PlugCircleExclamation, false);
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

    private StressMessageConfig LoadConfig()
    {
      string configFileLocation = Path.Combine(Environment.CurrentDirectory, "StressMessageConfig.json");

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

        UpdateServerState(ServerState.Connected, IconChar.UserCheck, Brushes.Lime, Brushes.Lime);

        _onHandleStatusBarState?.Invoke(IconChar.PlugCircleCheck, true);
        
        Console.WriteLine("Client connected!");
        
        SendStressMessage(config);
        
        _stressMessageTimer.Start();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Exception: {ex.Message}");
        
        UpdateServerState(ServerState.Error, IconChar.ExclamationTriangle, Brushes.Red, Brushes.OrangeRed);
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
        double randomValue = Random.Shared.NextDouble();

        var stressNotificationMessage = new StressNotificationMessage(
            (StressEffectCategory)Random.Shared.Next(0, 3),
            Math.Round(randomValue * 1.01, 2));

        string serializedMessage = JsonConvert.SerializeObject(stressNotificationMessage, new StringEnumConverter());
        
        byte[] messageBytes = Encoding.ASCII.GetBytes(serializedMessage);

        Console.WriteLine("Sending stress notification: " + serializedMessage);

        MessagesSent++;

        _clientSocket.Send(messageBytes);

        _onUpdateChartContent?.Invoke(stressNotificationMessage);
      }
      catch (SocketException ex)
      {
        Console.WriteLine($"SocketException: {ex.Message}");
      }
    }

    private void UpdateServerState(ServerState state, IconChar icon, Brush color, Brush iconColor)
    {
      _onServerStateChanged?.Invoke(state, icon, color, iconColor);
    }
  }
}