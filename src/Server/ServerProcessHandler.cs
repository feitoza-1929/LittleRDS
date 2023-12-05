public class ServerProcessHandler
{
    private readonly CommandsHandler _commandHandler;
    private readonly IRESPReader _respReader;
    private readonly IRESPWriter _respWriter;
    private readonly ServerPub _serverPub;

    public ServerProcessHandler(
        CommandsHandler commandsHandler,
        IRESPReader respReader,
        IRESPWriter respWriter,
        ServerPub serverPub)
    {
        _commandHandler = commandsHandler;
        _respReader = respReader;
        _respWriter = respWriter;
        _serverPub = serverPub;
    }
    public Value HandleRequest(string request)
    {
        Value clientCommandRequest = _respReader.Init(request);

        _serverPub.Emit(
            new() 
            { 
                Name = ServerEvents.SERVER_RECEIVED_COMMAND, 
                Data = new List<string>() { request, clientCommandRequest?.Array[0]?.Bulk ?? "" } 
            });
            
        return _commandHandler.HandleCommand(clientCommandRequest);
    }

    public void HandleResponse(Value respValue, Stream stream)
    {
        byte[] result = _respWriter.Init(respValue);
        stream.Write(result, 0, result.Length);
    }
}