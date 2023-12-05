using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Hosting;

public class Server : IHostedService
{
    private readonly int _port;
    private readonly IPAddress _localAddress;
    private readonly ServerPub _serverPub;
    private readonly ServerProcessHandler _processHandler;
    private TcpListener _listener;

    public Server(ServerPub serverPub, ServerProcessHandler processHandler)
    {
        _serverPub = serverPub;
        _processHandler = processHandler;
        _port = 6643;
        _localAddress = IPAddress.Parse("127.0.0.1");
        _listener = new TcpListener(_localAddress, _port);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _serverPub.Emit(new() { Name = ServerEvents.SERVER_STARTED, Data = _processHandler.HandleRequest});
        
        Listen(new byte[1024]);
        
        return Task.CompletedTask;
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
                    _processHandler.HandleResponse(_processHandler.HandleRequest(data),stream);
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

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Stop();
        return Task.CompletedTask;
    }

    private void Stop()
    {
        _serverPub.Emit(new() { Name = ServerEvents.SERVER_CLOSED });
        _listener.Stop();
    }
}