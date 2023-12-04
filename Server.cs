using System.Net;
using System.Net.Sockets;
using System.Text;

public class Server
{
    private readonly int _port;
    private readonly IPAddress _localAddress;
    private readonly TcpListener _listener;
    private readonly RESPReader _respReader;
    private readonly RESPWriter _respWriter;
    private readonly CommandsHandler _commandHandler;
    private readonly AOF _aof;

    public Server(IPAddress address, int port)
    {
        _localAddress = address;
        _port = port;
        _listener = new TcpListener(address, port);
        _respReader = new RESPReader();
        _respWriter = new RESPWriter();
        _commandHandler = new();
        _aof = new();
    }

    public void Start()
    {
        Console.WriteLine($"Sever starting...\n");
        _aof.Read(Process);
        _listener.Start();
        Listen(new byte[1024]);
    }

    private void Listen(byte[] buffer)
    {
        try
        {
            Console.WriteLine($"Sever started at {_localAddress}:{_port}\n");

            string? data;
            while (true)
            {
                using TcpClient client = _listener.AcceptTcpClient();
                using NetworkStream stream = client.GetStream();
                
                int i;

                while ((i = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    data = Encoding.ASCII.GetString(buffer, 0, i);
                    Response(Process(data), stream);
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

    private Value Process(string clientData)
    {
        Value clientCommandRequest = _respReader.Init(clientData);
        _aof.Write(clientData, clientCommandRequest?.Array[0]?.Bulk ?? "");
        return _commandHandler.HandleCommand(clientCommandRequest);
    }

    private void Response(Value respValue, Stream stream)
    {
        byte[] result = _respWriter.Init(respValue);
        stream.Write(result, 0, result.Length);
    }

    private void Stop()
    {
        _listener.Stop();
    }
}