using FontAwesome.Sharp;
using StressCommunicationAdminPanel.Models;
using System.Windows.Media;

namespace StressCommunicationAdminPanel.Refactoring
{
  public interface IStressMessageManager
  {
    int MessagesSent { get; set; }

    int MessagesReceived { get; set; }
    
    string DeviceName { get; set; }
    
    void ManageServerState();
    
    void StopServer();
    
    void StartServer();

    void UpdateServerState(ServerState state, IconChar icon, Brush color, Brush iconColor);
  }
}