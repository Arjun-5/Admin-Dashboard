using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StressCommunicationAdminPanel.Refactoring
{
  public class TcpCommunicationHandler
  {
    private Socket _serverSocket;

    private Socket _clientSocket;

    public async Task<Socket> SetupTcpServer(string ipAddress, int port, CancellationToken cancellationToken)
    {
      _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      
      _serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port));
      
      _serverSocket.Listen(2);

      Console.WriteLine("Server is listening for connections");

      _clientSocket = await Task.Run(() => _serverSocket.Accept(), cancellationToken);
      
      return _clientSocket;
    }

    public async Task ReceiveMessagesFromClient(Action<string> onMessageReceived, CancellationToken cancellationToken)
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        byte[] buffer = new byte[1024];
        
        int bytesReceived = await _clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
        
        string clientMessage = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
        
        onMessageReceived(clientMessage);
      }
    }

    public void Close()
    {
      _clientSocket?.Close();
      
      _serverSocket?.Close();
    }
  }
}