namespace LittleRDS.Persistence;

public class AOF
{
    private static bool _isReading;
    private static string _defaultPath = Path.GetFullPath("/Persistence/Data/aof.txt");

    public static void Read(Func<string, object> processCommands)
    {
        _isReading = true;

        string? sourceRESP = FileIO.Read(_defaultPath);

        if (sourceRESP is null)
        {
            _isReading = false;
            return;
        }
            

        string[] commands = sourceRESP.Split("[end]");

        foreach (var command in commands)
        {
            if (!string.IsNullOrEmpty(command))
                processCommands(command);
        }

        _isReading = false;
    }

    public static void Write(string command, string commandType)
    {
        if (_isReading || commandType.ToUpper() == "GET" || commandType.ToUpper() == "HGET")
            return;

        FileIO.Write(_defaultPath, $"{command}[end]");
    }
}