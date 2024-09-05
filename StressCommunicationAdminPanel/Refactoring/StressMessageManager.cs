using FontAwesome.Sharp;
using StressCommunicationAdminPanel.Models;
using System;
using System.Windows.Media;
using System.Threading;
using System.Threading.Tasks;
using StressCommunicationAdminPanel.Services;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;

namespace StressCommunicationAdminPanel.Refactoring
{
  public class StressMessageManager : PropertyChangeHandler, IStressMessageManager
  {
    private VRAppMessageHandler _vrAppMessageCommunicationHandler;

    private ExternalAppStressMessageHandler _externalAppMessageCommunicationHandler;

    private Action<ServerState, IconChar, Brush, Brush> onServerStateChanged;

    private Action<StressNotificationMessage> onUpdateStressMessageChart;

    private Action<IconChar, bool> onHandleStatusBarState;

    private Action<MessageTypeInfo> onUpdateVRDataChartContent;

    private CancellationTokenSource _vrAppCancellationTokenSource = new CancellationTokenSource();

    private CancellationTokenSource _stressMessageExternalAppTokenSource = new CancellationTokenSource();

    private string _deviceName;

    private bool _serverRunning;

    public string DeviceName
    {
      get => _deviceName;

      set
      {
        _deviceName = value;

        OnPropertyChanged(nameof(DeviceName));
      }
    }

    public int MessagesSent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int MessagesReceived { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public StressMessageManager(Action<ServerState, IconChar, Brush, Brush> onServerStateChanged, Action<StressNotificationMessage> onUpdateChartContent, Action<IconChar, bool> onHandleStatusBarState, Action<MessageTypeInfo> onUpdateVRDataChartContent)
    {
      this.onServerStateChanged = onServerStateChanged;

      onUpdateStressMessageChart = onUpdateChartContent;

      this.onHandleStatusBarState = onHandleStatusBarState;

      this.onUpdateVRDataChartContent = onUpdateVRDataChartContent;

      DeviceName = ConfigurationManager.AppSettings.Get("DefaultClientNameValue");

      _vrAppMessageCommunicationHandler = new();
    }
    public async void ManageServerState()
    {
      if(_serverRunning)
      {
        var stressNotificationMessage = new StressNotificationMessage()
        {
          currentStressCategory = StressEffectCategory.None,
          stressLevel = -1,
          cancellationStatus = !_serverRunning
        };

        string serializedMessage = JsonConvert.SerializeObject(stressNotificationMessage, new StringEnumConverter());

        byte[] serializedMessageBytes = Encoding.ASCII.GetBytes(serializedMessage);

        await _vrAppMessageCommunicationHandler?.VRAppCommunicationSocket.SendAsync(serializedMessageBytes);
        
        StopServer();
      
        _vrAppCancellationTokenSource.Cancel();

        _vrAppCancellationTokenSource.Dispose();
      }
      else
      {
        StartServer();
      }
    }
    public async void StartServer()
    {
      _serverRunning = true;

      UpdateServerState(ServerState.Starting, IconChar.UserClock, Brushes.OrangeRed, Brushes.OrangeRed);

      await _externalAppMessageCommunicationHandler.BroadcastMessageHandler.SendBroadcastMessage();
    }
    public void StopServer()
    {
      throw new NotImplementedException();
    }

    public void UpdateServerState(ServerState state, IconChar icon, Brush color, Brush iconColor)
    {
      throw new NotImplementedException();
    }
  }
}