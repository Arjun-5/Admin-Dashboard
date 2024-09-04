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
using System.Threading;
using Timer = System.Timers.Timer;
using StressCommunicationAdminPanel.Helpers;
using System.Windows;
using System.Windows.Threading;

namespace StressCommunicationAdminPanel.Services
{
  public class StressMessageManager : PropertyChangeHandler
  {
    private UdpClient _client;

    private Socket _stressMessageSocket;

    private Socket _vrAppClientSocket;
    
    private Timer _stressMessageTimer;
    
    private Action<ServerState, IconChar, Brush, Brush> _onServerStateChanged;

    private Action<StressNotificationMessage> _onUpdateChartContent;

    private Action<MessageTypeInfo> _onUpdateReceivedDataChartContent;

    private Action<IconChar, bool> _onHandleStatusBarState;

    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    private CancellationTokenSource _stressMessageExternalAppTokenSource = new CancellationTokenSource();

    private bool _serverRunning;

    private int _messagesSent;

    private int _messagesReceived;

    private string _deviceName;
    public int MessagesSent
    {
      get => _messagesSent;

      set
      {
        _messagesSent = value;

        OnPropertyChanged(nameof(MessagesSent));
      }
    }
    public int MessagesReceived
    {
      get => _messagesReceived;

      set
      {
        _messagesReceived = value;

        OnPropertyChanged(nameof(MessagesReceived));
      }
    }
    public string DeviceName
    {
      get => _deviceName;

      set
      {
        _deviceName = value;

        OnPropertyChanged(nameof(DeviceName));
      }
    }
    //Need to update setup later
    public StressMessageManager(Action<ServerState, IconChar, Brush, Brush> onServerStateChanged, Action<StressNotificationMessage> onUpdateChartContent, Action<IconChar, bool> onHandleStatusBarState, Action<MessageTypeInfo> onUpdateReceivedChartContent)
    {
      _onServerStateChanged = onServerStateChanged;

      _onUpdateChartContent = onUpdateChartContent;

      _onHandleStatusBarState = onHandleStatusBarState;

      _onUpdateReceivedDataChartContent = onUpdateReceivedChartContent;

      DeviceName = "Not Connected";
    }

    public async void ManageServerState()
    {
      if (_serverRunning)
      {
        var stressNotificationMessage = new StressNotificationMessage(StressEffectCategory.None, -1, !_serverRunning);

        string serializedMessage = JsonConvert.SerializeObject(stressNotificationMessage, new StringEnumConverter());

        byte[] messageBytes = Encoding.ASCII.GetBytes(serializedMessage);

        await _vrAppClientSocket.SendAsync(messageBytes);

        StopServer();

        _cancellationTokenSource.Cancel();

        _cancellationTokenSource.Dispose();
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
      
      var config = ConfigHandler.LoadConfig();

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

      try
      {
        _stressMessageTimer?.Stop();

        _vrAppClientSocket?.Shutdown(SocketShutdown.Both);

        _vrAppClientSocket?.Close();

        _client?.Close();
      }
      catch (Exception ex)
      {
        Console.WriteLine("Exception occured when disposing socket object" + ex.Message);
      }

      UpdateServerState(ServerState.Stopped, IconChar.UserTimes, Brushes.Red, Brushes.OrangeRed);

      _onHandleStatusBarState?.Invoke(IconChar.PlugCircleExclamation, false);
    }
    private async void ConfigureStressMessageSocketAttributes(StressMessageConfig config)
    {
      var stressMessageSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

      stressMessageSocket.Bind(new IPEndPoint(IPAddress.Loopback, config.stressMessageSendingPort));

      stressMessageSocket.Listen();

      _stressMessageSocket = await Task.Run(() => stressMessageSocket.Accept(), _stressMessageExternalAppTokenSource.Token);

      if (_stressMessageSocket == null)
      {
        Console.WriteLine("Failed to accept a client connection.");

        return;
      }

      await Task.Run(() =>
      {
        ReceiveStressMessagesFromExternalApp(_stressMessageExternalAppTokenSource.Token);
      }, _stressMessageExternalAppTokenSource.Token);
    }
    private async void ReceiveStressMessagesFromExternalApp(CancellationToken cancellationToken)
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        try
        {
          byte[] buffer = new byte[1024];
          
          int bytesReceived = await _stressMessageSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

          string message = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

          var stressNotificationMessage = JsonConvert.DeserializeObject<StressNotificationMessage>(message);

          Console.WriteLine("The stress notification message : " + message);

          if (stressNotificationMessage != null)
          {
            MessagesReceived++;
            
            _onUpdateChartContent?.Invoke(stressNotificationMessage);

            if (_vrAppClientSocket == null)
            {
              return;
            }

            try
            {
              byte[] messageBytes = Encoding.ASCII.GetBytes(message);

              _vrAppClientSocket.Send(messageBytes);
              
              MessagesSent++;
            }
            catch (SocketException ex)
            {
              Console.WriteLine($"SocketException: {ex.Message}");

              _stressMessageExternalAppTokenSource.Cancel();
            }
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Exception {ex.Source} Occurred with the following message : {ex.Message}");

          _stressMessageExternalAppTokenSource.Cancel();
        }
      }
    }
    private async void SendBroadcastMessage()
    {
      _client = new UdpClient();
      
      IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 11000);
      
      byte[] message = Encoding.ASCII.GetBytes("Server IP Address Broadcast Message");
      
      _client.Send(message, message.Length, endPoint);
      
      await ReceiveClientInformationMessage();

      MessagesReceived++;

      _client.Close();
    }

    private async Task SetupConnectionParameters(StressMessageConfig config)
    {
      try
      {
        var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        serverSocket.Bind(new IPEndPoint(IPAddress.Parse(config.ipAddress), config.stressMessageSendingPort));
        
        serverSocket.Listen(2);

        Console.WriteLine("Server is listening for connections");

        await Task.Run(() =>
        {
          ConfigureStressMessageSocketAttributes(config);
        }, _stressMessageExternalAppTokenSource.Token);

        _vrAppClientSocket = await Task.Run(() =>
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
        },_cancellationTokenSource.Token);

        if (_vrAppClientSocket == null)
        {
          Console.WriteLine("Failed to accept a client connection.");
          
          return;
        }

        if (config.shouldUseDebugSetup)
        {
          _stressMessageTimer = new Timer(config.messageTimeInterval);

          _stressMessageTimer.Elapsed += (sender, e) => OnStressMessageTimerElapsed(sender, e, config);

          SendStressMessage(config);

          _stressMessageTimer.Start();
        }

        UpdateServerState(ServerState.Connected, IconChar.UserCheck, Brushes.Lime, Brushes.Lime);

        _onHandleStatusBarState?.Invoke(IconChar.PlugCircleCheck, true);

        await Task.Run(() =>
        {
          ReceiveMessagesFromClient(_cancellationTokenSource.Token);
        }, _cancellationTokenSource.Token);
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
      if (_vrAppClientSocket == null)
      {
        return;
      }

      try
      {
        double randomValue = Random.Shared.NextDouble();

        var stressNotificationMessage = new StressNotificationMessage(
            (StressEffectCategory)Random.Shared.Next(1, 4),
            Math.Round(randomValue * 1.01, 2), !_serverRunning);

        string serializedMessage = JsonConvert.SerializeObject(stressNotificationMessage, new StringEnumConverter());
        
        byte[] messageBytes = Encoding.ASCII.GetBytes(serializedMessage);

        Console.WriteLine("Sending stress notification: " + serializedMessage);

        MessagesSent++;

        _vrAppClientSocket.Send(messageBytes);

        _onUpdateChartContent?.Invoke(stressNotificationMessage);
      }
      catch (SocketException ex)
      {
        Console.WriteLine($"SocketException: {ex.Message}");
      }
    }
    private async Task ReceiveClientInformationMessage()
    {
      try
      {
        byte[] buffer = new byte[1024];

        var udpReceiveResult = await _client.ReceiveAsync();

        string clientMessage = Encoding.ASCII.GetString(udpReceiveResult.Buffer);

        var deviceInfo = JsonConvert.DeserializeObject<ReceivedMessageInfo>(clientMessage);

        DeviceName = deviceInfo.messageContent;

        _onUpdateReceivedDataChartContent?.Invoke(MessageTypeInfo.DeviceInfo);
      }
      catch (SocketException ex)
      {
        Console.WriteLine($"SocketException: {ex.Message}");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Exception Occured: {ex.Message}");
      }
    }
    private async void ReceiveMessagesFromClient(CancellationToken cancellationToken)
    {
      while(!cancellationToken.IsCancellationRequested)
      {
        try
        {
          byte[] buffer = new byte[1024];

          int bytesReceived = await _vrAppClientSocket.ReceiveAsync(new ArraySegment<byte>(buffer),SocketFlags.None);

          string clientMessage = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

          var receivedMessage = JsonConvert.DeserializeObject<ReceivedMessageInfo>(clientMessage);

          if (receivedMessage != null)
          {
            MessagesReceived++;

            if (receivedMessage.MessageTypeInfo == MessageTypeInfo.SelfReportStressInfo) 
            {
              _onUpdateReceivedDataChartContent?.Invoke(MessageTypeInfo.SelfReportStressInfo);
            }

            if (receivedMessage.MessageTypeInfo == MessageTypeInfo.Exit)
            {
              _cancellationTokenSource.Cancel();
            }
          }
        }
        catch (SocketException ex)
        {
          Console.WriteLine($"SocketException: {ex.Message}");
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Exception Occured: {ex.Message}");
        }
      }
      _ =   Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => StopServer() ));
    }
    private void UpdateServerState(ServerState state, IconChar icon, Brush color, Brush iconColor)
    {
      _onServerStateChanged?.Invoke(state, icon, color, iconColor);
    }
  }
}