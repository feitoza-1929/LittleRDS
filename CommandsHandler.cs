using System.Net.NetworkInformation;

public class CommandsHandler
{
    private Value[] _commandArguments;
    private readonly Dictionary<string, Func<Value>> _commands;
    private Dictionary<string, string> _sets;

    public CommandsHandler()
    {
        _sets = new();
        _commandArguments = new Value[] { new() { Type = nameof(DataTypes.NULL) } };
        _commands = new() { 
            { "PING", () => Ping(_commandArguments) },
            { "SET", () => Set(_commandArguments) },
            { "GET", () => Get(_commandArguments) }
        };
    }

    public Value HandleCommand(Value commandArguments)
    {
        // if (commandArguments.Array[0].Bulk.ToUpper() != "PING")
        //     return new Value();

        _commandArguments = commandArguments.Array[1..];
        return _commands[commandArguments.Array[0].Bulk.ToUpper()]();
    }
    
    private Value Ping(Value[] commandArguments)
    {
        return commandArguments.Length > 0 
            ? new Value() { Type = nameof(DataTypes.STRING), String = commandArguments[0].Bulk } 
            : new Value() { Type = nameof(DataTypes.STRING), String = "PONG"};
    }

    private Value Set(Value[] commandArguments)
    {
        _sets.Add(commandArguments[0].Bulk, commandArguments[1].Bulk);
        return new Value() { Type = nameof(DataTypes.STRING), String = "OK" };
    }

    private Value Get(Value[] commandArguments)
    {
        return new Value() { Type = nameof(DataTypes.BULK), Bulk = _sets[commandArguments[0].Bulk] };
    }
}