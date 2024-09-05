using Newtonsoft.Json;
using StressCommunicationAdminPanel.Models;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StressCommunicationAdminPanel.Refactoring
{
  public class ExternalAppStressMessageHandler
  {
    private Socket _stressMessageExternalAppSocket;

    private CancellationTokenSource _stressMessageExternalAppTokenSource;

    private UdpCommunicationHandler _broadcastMessageHandler;

    private Action<StressNotificationMessage> onStressNotificationMessageReceived;

    public UdpCommunicationHandler BroadcastMessageHandler => _broadcastMessageHandler;

    public ExternalAppStressMessageHandler()
    {
      _broadcastMessageHandler = new();
    }
    private async void ConfigureSocketConnectionAttributes(StressMessageConfig appConfig)
    {
      var stressMessageReceiverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

      stressMessageReceiverSocket.Bind(new IPEndPoint(IPAddress.Loopback, appConfig.stressMessageSendingPort));

      stressMessageReceiverSocket.Listen();

      _stressMessageExternalAppSocket = await Task.Run(async() =>
      {
        try
        {
          return await stressMessageReceiverSocket.AcceptAsync();
        }
        catch (Exception e)
        {
          Console.WriteLine($"Exception [{e.Source}] occurred with the following message : {e.Message}");

          return null;
        }
      },_stressMessageExternalAppTokenSource.Token);

      if (_stressMessageExternalAppSocket == null)
      {
        Console.WriteLine("Error when trying to accept a connection from the External Stress Message App");

        return;
      }

      await Task.Run(() =>
      {
        ReceiveStressMessagesFromExternalApp();
      }, _stressMessageExternalAppTokenSource.Token);
    }
    private async void ReceiveStressMessagesFromExternalApp()
    {
      while (!_stressMessageExternalAppTokenSource.IsCancellationRequested)
      {
        try
        {
          byte[] messageBuffer = new byte[1024];

          int bytesReceived = await _stressMessageExternalAppSocket.ReceiveAsync(new ArraySegment<byte>(messageBuffer), SocketFlags.None);

          string rawStressMessageData = Encoding.ASCII.GetString(messageBuffer, 0, bytesReceived);

          var stressNotificationMessage = JsonConvert.DeserializeObject<StressNotificationMessage>(rawStressMessageData);

          Console.WriteLine($"The Deserialized stress notification message is --: {stressNotificationMessage}");

          if(stressNotificationMessage != null)
          {
            onStressNotificationMessageReceived?.Invoke(stressNotificationMessage);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Exception {ex.Source} with the following message : {ex.Message}");
        
          _stressMessageExternalAppTokenSource.Cancel();
        }
      }
    }
  }
}