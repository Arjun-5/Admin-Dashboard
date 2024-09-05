using Newtonsoft.Json;
using StressCommunicationAdminPanel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StressCommunicationAdminPanel.Refactoring
{
  public class UdpCommunicationHandler
  {
    private UdpClient _udpClient;

    public async Task SendBroadcastMessage()
    {
      _udpClient = new UdpClient();

      IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 11000);
      
      byte[] message = Encoding.ASCII.GetBytes("Server IP Address Broadcast Message");
      
      await _udpClient.SendAsync(message, message.Length, endPoint);
    }

    public async Task<ReceivedMessageInfo> ReceiveClientInformationMessage()
    {
      try
      {
        var udpReceiveResult = await _udpClient.ReceiveAsync();
        
        string clientMessage = Encoding.ASCII.GetString(udpReceiveResult.Buffer);
        
        return JsonConvert.DeserializeObject<ReceivedMessageInfo>(clientMessage);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Exception Occurred: {ex.Message}");
        
        return null;
      }
    }

    public void Close()
    {
      _udpClient.Close();
    }
  }
}
