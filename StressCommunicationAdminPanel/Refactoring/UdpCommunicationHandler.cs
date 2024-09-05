using Newtonsoft.Json;
using StressCommunicationAdminPanel.Models;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StressCommunicationAdminPanel.Refactoring
{
  public class UdpCommunicationHandler
  {
    private UdpClient _udpClient;

    private Action<MessageTypeInfo> onStressNotificationMessageSent;

    public async Task SendBroadcastMessage()
    {
      _udpClient = new UdpClient();

      IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 11000);
      
      byte[] message = Encoding.ASCII.GetBytes("Server IP Address Broadcast Message");
      
      await _udpClient.SendAsync(message, message.Length, endPoint);

      await ReceiveClientInformationMessage();

      Close();
    }

    public async Task ReceiveClientInformationMessage()
    {
      try
      {
        var udpReceiveResult = await _udpClient.ReceiveAsync();
        
        string clientMessage = Encoding.ASCII.GetString(udpReceiveResult.Buffer);
        
         JsonConvert.DeserializeObject<ReceivedMessageInfo>(clientMessage);

        onStressNotificationMessageSent?.Invoke(MessageTypeInfo.DeviceInfo);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Exception {ex.Source} occured with the following message {ex.Message}");

        return;
      }
    }

    public void Close()
    {
      _udpClient?.Close();
    }
  }
}