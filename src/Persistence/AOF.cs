public class AOF
{
    private bool _isReading;
    private string defaultPath = Path.GetFullPath("aof.txt");
    public void Read(Func<string, object> processCommands)
    {
        if (!Path.Exists(defaultPath))
            return;

        _isReading = true;

        string? sourceRESP = FileIO.Read(defaultPath);
        string[]? commands = sourceRESP?.Split("[end]");

        foreach (var command in commands)
        {
            if (!string.IsNullOrEmpty(command))
                processCommands(command);
        }

        _isReading = false;
    }
    public void Write(string command, string commandType)
    {
        if (_isReading || commandType.ToUpper() == "GET" || commandType.ToUpper() == "HGET")
            return;

        FileIO.Write(defaultPath, $"{command}[end]");
    }
}