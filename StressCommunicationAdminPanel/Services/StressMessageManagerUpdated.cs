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
  public class StressMessageManagerUpdated : PropertyChangeHandler
  {
    private UdpClient _client;
    private Socket _clientSocket;
    private Socket _unityClientSocket;
    private Timer _stressMessageTimer;
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private Action<ServerState, IconChar, Brush, Brush> _onServerStateChanged;
    private Action<StressNotificationMessage> _onUpdateChartContent;
    private Action<MessageTypeInfo> _onUpdateReceivedDataChartContent;
    private Action<IconChar, bool> _onHandleStatusBarState;

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

    public StressMessageManagerUpdated(Action<ServerState, IconChar, Brush, Brush> onServerStateChanged, Action<StressNotificationMessage> onUpdateChartContent, Action<IconChar, bool> onHandleStatusBarState, Action<MessageTypeInfo> onUpdateReceivedChartContent)
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
        _clientSocket?.Shutdown(SocketShutdown.Both);
        _clientSocket?.Close();
        _unityClientSocket?.Shutdown(SocketShutdown.Both);
        _unityClientSocket?.Close();
        _client?.Close();
      }
      catch (Exception ex)
      {
        Console.WriteLine("Exception occurred when disposing socket object" + ex.Message);
      }

      UpdateServerState(ServerState.Stopped, IconChar.UserTimes, Brushes.Red, Brushes.OrangeRed);
      _onHandleStatusBarState?.Invoke(IconChar.PlugCircleExclamation, false);
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

        // Accept connection from Python app
        _clientSocket = await Task.Run(() => serverSocket.Accept(), _cancellationTokenSource.Token);

        if (_clientSocket == null)
        {
          Console.WriteLine("Failed to accept a client connection.");
          return;
        }

        Console.WriteLine("Client connected!");

        // Accept connection from Unity VR app
        _unityClientSocket = await Task.Run(() => serverSocket.Accept(), _cancellationTokenSource.Token);

        if (_unityClientSocket == null)
        {
          Console.WriteLine("Failed to accept a Unity VR app connection.");
          return;
        }

        UpdateServerState(ServerState.Connected, IconChar.UserCheck, Brushes.Lime, Brushes.Lime);
        _onHandleStatusBarState?.Invoke(IconChar.PlugCircleCheck, true);

        // Start listening for messages from Python app
        await Task.Run(() =>
        {
          ReceiveMessagesFromPythonApp(_cancellationTokenSource.Token);
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
      // No longer needed since stress messages come from Python app
    }

    private async void ReceiveMessagesFromPythonApp(CancellationToken cancellationToken)
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        try
        {
          byte[] buffer = new byte[1024];
          int bytesReceived = await _clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
          string pythonMessage = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

          var stressNotificationMessage = JsonConvert.DeserializeObject<StressNotificationMessage>(pythonMessage);

          if (stressNotificationMessage != null)
          {
            MessagesReceived++;
            _onUpdateChartContent?.Invoke(stressNotificationMessage);

            // Forward the message to Unity VR app
            ForwardMessageToUnity(pythonMessage);
          }
        }
        catch (SocketException ex)
        {
          Console.WriteLine($"SocketException: {ex.Message}");
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Exception Occurred: {ex.Message}");
        }
      }
      Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => StopServer()));
    }

    private void ForwardMessageToUnity(string message)
    {
      if (_unityClientSocket == null)
      {
        return;
      }

      try
      {
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        _unityClientSocket.Send(messageBytes);
        MessagesSent++;
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
        Console.WriteLine($"Exception Occurred: {ex.Message}");
      }
    }

    private void UpdateServerState(ServerState state, IconChar icon, Brush color, Brush iconColor)
    {
      _onServerStateChanged?.Invoke(state, icon, color, iconColor);
    }
  }
}