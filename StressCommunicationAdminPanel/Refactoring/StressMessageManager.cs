using FontAwesome.Sharp;
using StressCommunicationAdminPanel.Models;
using System;
using System.Windows.Media;
using System.Threading;
using System.Threading.Tasks;

namespace StressCommunicationAdminPanel.Refactoring
{
  public class StressMessageManager : IStressMessageManager
  {
    private UdpCommunicationHandler _udpHandler;
    
    private TcpCommunicationHandler _tcpHandler;

    private Timer _stressMessageTimer;
    
    private bool _serverRunning;
    
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public int MessagesSent { get; set; }
    
    public int MessagesReceived { get; set; }
    
    public string DeviceName { get; set; }

    private Action<ServerState, IconChar, Brush, Brush> _onServerStateChanged;
    
    private Action<StressNotificationMessage> _onUpdateChartContent;

    public StressMessageManager(Action<ServerState, IconChar, Brush, Brush> onServerStateChanged, Action<StressNotificationMessage> onUpdateChartContent)
    {
      _udpHandler = new UdpCommunicationHandler();
      
      _tcpHandler = new TcpCommunicationHandler();
      
      _onServerStateChanged = onServerStateChanged;
      
      _onUpdateChartContent = onUpdateChartContent;
      
      DeviceName = "Not Connected";
    }

    public async void ManageServerState()
    {
      if (_serverRunning)
      {
        StopServer();
      }
      else
      {
        await StartServer();
      }
    }

    private async Task StartServer()
    {
      _serverRunning = true;
      
      UpdateServerState(ServerState.Starting, IconChar.UserClock, Brushes.OrangeRed, Brushes.OrangeRed);

      await _udpHandler.SendBroadcastMessage();
      
      var clientInfo = await _udpHandler.ReceiveClientInformationMessage();

      if (clientInfo != null)
      {
        DeviceName = clientInfo.messageContent;
        
        UpdateServerState(ServerState.Connected, IconChar.UserCheck, Brushes.Lime, Brushes.Lime);
      }
    }

    private void StopServer()
    {
      _serverRunning = false;
      
      _udpHandler.Close();
      
      _tcpHandler.Close();
      
      _cancellationTokenSource.Cancel();
      
      UpdateServerState(ServerState.Stopped, IconChar.UserTimes, Brushes.Red, Brushes.OrangeRed);
    }

    private void UpdateServerState(ServerState state, IconChar icon, Brush color, Brush iconColor)
    {
      _onServerStateChanged?.Invoke(state, icon, color, iconColor);
    }
  }
}