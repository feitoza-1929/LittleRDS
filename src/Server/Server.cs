using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using LittleRDS.PubSub.Publishers;

namespace LittleRDS.Server;

public class Server
{
    private readonly int _port;
    private readonly IPAddress _localAddress;
    private readonly ServerPub _serverPub;
    private readonly ProcessHandler _processHandler;
    private TcpListener _listener;

    public Server()
    {
        _serverPub = ServerPub.GetInstance();
        _processHandler = new ProcessHandler();
        _port = 6643;
        _localAddress = IPAddress.Parse("127.0.0.1");
        _listener = new TcpListener(_localAddress, _port);
    }

    public void Start()
    {
        _serverPub.Emit(new() { Name = ServerEvents.SERVER_STARTED, Data = _processHandler.HandleRequest});
        Listen(new byte[1024]);
    }

    private void Listen(byte[] buffer)
    {
        try
        {
            _listener.Start();
            
            string? data;
            while (true)
            {
                using TcpClient client = _listener.AcceptTcpClient();
                using NetworkStream stream = client.GetStream();
                
                int i;

                while ((i = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    data = Encoding.ASCII.GetString(buffer, 0, i);
                    _processHandler.HandleResponse(
                        _processHandler.HandleRequest(data),stream);
                }
            }
        }
        catch(SocketException ex)
        {
            Console.WriteLine($"SocketException: {ex}");
        }
        finally
        {
            Stop();
        }
    }
    private void Stop()
    {
        _serverPub.Emit(new() { Name = ServerEvents.SERVER_CLOSED });
        _listener.Stop();
    }
}