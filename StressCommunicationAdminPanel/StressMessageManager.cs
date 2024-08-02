using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Timers;
using System.IO;
using System.Collections.Generic;
using StressCommunicationAdminPanel.Model;

namespace StressCommunicationAdminPanel
{
  public class StressMessageManager
  {
    public void SendBroadcastMessage()
    {
      UdpClient client = new UdpClient();

      IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 11000);

      byte[] message = Encoding.ASCII.GetBytes("Server IP Address Broadcast Message");

      client.Send(message, message.Length, endPoint);

      client.Close();

      SetupConnectionParameters(ProcessAndGetConfigInformation());
    }
    private StressMessageConfig ProcessAndGetConfigInformation()
    {
      string currentWorkingDirectory = Environment.CurrentDirectory;

      string configFileLocation = Path.Combine(currentWorkingDirectory, "StressMessageConfig.json");

      var processedConfigData = File.ReadAllText(configFileLocation);

      return JsonConvert.DeserializeObject<StressMessageConfig>(processedConfigData);
    }
    private void SetupConnectionParameters(StressMessageConfig stressMessageConfig)
    {
      if (stressMessageConfig == null)
      {
        return;
      }

      var stressMessageTimer = new Timer(stressMessageConfig.messageTimeInterval);

      stressMessageTimer.Enabled = true;

      bool executeAtStart = false;

      Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

      serverSocket.Bind(new IPEndPoint(IPAddress.Parse(stressMessageConfig.ipAddress), stressMessageConfig.stressMessageSendingPort));

      serverSocket.Listen(1);

      Socket clientSocket = serverSocket.Accept();

      stressMessageTimer.Elapsed += (sender, e) => OnStressMessageTimerElapsed(sender, e, clientSocket, stressMessageConfig.messageTimeInterval);

      while (true)
      {
        if (!executeAtStart)
        {
          executeAtStart = true;

          SendStressMessage(clientSocket, stressMessageConfig.messageTimeInterval);
        }
        GetClientMessage(clientSocket);
      }
    }
    private void OnStressMessageTimerElapsed(object sender, ElapsedEventArgs e, Socket clientSocket, int seconds)
    {
      SendStressMessage(clientSocket, seconds);
    }
    private void SendStressMessage(Socket clientSocket, int seconds)
    {
      if (clientSocket == null)
      {
        return;
      }

      StressNotificationMessage stressNotificationMessage = new StressNotificationMessage(
        currentStressEffect: (StressEffectType)new Random().Next(0, 4),
        stressLevel: new Random().Next(0, 1));

      string serializedNotificationMessage = JsonConvert.SerializeObject(stressNotificationMessage, new StringEnumConverter());

      byte[] stressInfoMessage = Encoding.ASCII.GetBytes(serializedNotificationMessage);

      clientSocket.Send(stressInfoMessage);

    }
    private void GetClientMessage(Socket clientSocket)
    {
      byte[] bytes = new byte[1024];

      int clientBytes = clientSocket.Receive(bytes);

      string clientMessage = Encoding.ASCII.GetString(bytes, 0, clientBytes);
    }
  }
}
