using Newtonsoft.Json;
using StressCommunicationAdminPanel.Models;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace StressCommunicationAdminPanel.Refactoring
{
  public class VRAppMessageHandler
  {
    private Socket _vrAppCommunicationSocket;

    private CancellationTokenSource _stressMessageExternalAppTokenSource;

    private Action<MessageTypeInfo> onReceiveDataFromVRApp;
    
    public Socket VRAppCommunicationSocket => _vrAppCommunicationSocket;
    public VRAppMessageHandler() 
    {
      
    }
    private async void SendStressMessagesToVRApp(string rawStressMessageData)
    {
      if(_vrAppCommunicationSocket == null)
      {
        Console.WriteLine("The VR App communication is not established properly. Check the socket configuration");

        return;
      }

      try
      {
        byte[] messageBytes = Encoding.ASCII.GetBytes(rawStressMessageData);

        await _vrAppCommunicationSocket.SendAsync(messageBytes);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Exception {ex.Source} occured with the following message {ex.Message}");

        _stressMessageExternalAppTokenSource.Cancel();
      }
    }
    private async void ReceiveMessagesFromClient()
    {
      while (!_stressMessageExternalAppTokenSource.IsCancellationRequested)
      {
        try
        {
          byte[] messageBuffer = new byte[1024];

          int bytesReceived = await _vrAppCommunicationSocket.ReceiveAsync(new ArraySegment<byte>(messageBuffer), SocketFlags.None);

          string rawVRAppMessage = Encoding.ASCII.GetString(messageBuffer, 0, bytesReceived);

          var vrAppMessage = JsonConvert.DeserializeObject<ReceivedMessageInfo>(rawVRAppMessage);

          if (vrAppMessage != null)
          {
            switch (vrAppMessage.MessageTypeInfo)
            {
              case MessageTypeInfo.SelfReportStressInfo:
              {
                onReceiveDataFromVRApp?.Invoke(MessageTypeInfo.SelfReportStressInfo);
                break;
              }
              case MessageTypeInfo.Exit:
              {
                _stressMessageExternalAppTokenSource.Cancel();
                break;
              }
              default:
                break;
            }
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Exception {ex.Source} occured with the following message {ex.Message}");

          _stressMessageExternalAppTokenSource.Cancel();
        }
      }
    }
  }
}
