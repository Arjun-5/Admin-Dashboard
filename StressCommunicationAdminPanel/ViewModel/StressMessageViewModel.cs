using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StressCommunicationAdminPanel.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;

namespace StressCommunicationAdminPanel.ViewModel
{
  public class StressMessageViewModel : AppViewModel
  {
    private ObservableCollection<StressMessage> _messages;
    
    private UdpClient _client;
    
    private Timer _stressMessageTimer;

    private Socket _clientSocket;

    public ObservableCollection<StressMessage> Messages
    {
      get { return _messages; }

      set { _messages = value; OnPropertyChanged(nameof(Messages)); }
    }

    public ICommand SendBroadcastMessageCommand { get; }

    public StressMessageViewModel()
    {
      _messages = new ObservableCollection<StressMessage>();

      SendBroadcastMessageCommand = new RelayCommand(SendBroadcastMessage);
    }

    private void SendBroadcastMessage()
    {
      _client = new UdpClient();

      IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 11000);
      
      byte[] message = Encoding.ASCII.GetBytes("Server IP Address Broadcast Message");
      
      _client.Send(message, message.Length, endPoint);
      
      _client.Close();

      SetupConnectionParameters(ProcessAndGetConfigInformation());
    }
    private StressMessageConfig ProcessAndGetConfigInformation()
    {
      string currentWorkingDirectory = Environment.CurrentDirectory;
      
      string configFileLocation = Path.Combine(currentWorkingDirectory, "StressMessageConfig.json");
      
      var processedConfigData = File.ReadAllText(configFileLocation);
      
      return JsonConvert.DeserializeObject<StressMessageConfig>(processedConfigData);
    }

    private void SetupConnectionParameters(StressMessageConfig config)
    {
      if (config == null)
      {
        return;
      }

      _stressMessageTimer = new Timer(config.messageTimeInterval);

      _stressMessageTimer.Elapsed += (sender, e) => OnStressMessageTimerElapsed(sender, e, config);
      
      _stressMessageTimer.Start();

      var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      
      serverSocket.Bind(new IPEndPoint(IPAddress.Parse(config.ipAddress), config.stressMessageSendingPort));
      
      serverSocket.Listen(1);

      _clientSocket = serverSocket.Accept();
      
      SendStressMessage(config);
    }
    private void OnStressMessageTimerElapsed(object sender, ElapsedEventArgs e, StressMessageConfig config)
    {
      SendStressMessage(config);
    }
    private void SendStressMessage(StressMessageConfig config)
    {
      if (_clientSocket == null)
      {
        return;
      }

      var stressNotificationMessage = new StressNotificationMessage(
          (StressEffectType)new Random().Next(0, 4),
          new Random().Next(0, 2)
      );

      string serializedMessage = JsonConvert.SerializeObject(stressNotificationMessage, new StringEnumConverter());
      
      byte[] messageBytes = Encoding.ASCII.GetBytes(serializedMessage);
      
      _clientSocket.Send(messageBytes);

      Messages.Add(new StressMessage
      {
        TimeSent = DateTime.Now,
        Message = serializedMessage
      });
    }
  }
}