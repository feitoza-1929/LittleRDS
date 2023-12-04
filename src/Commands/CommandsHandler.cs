using System.Net.NetworkInformation;
using System.Xml.XPath;

public class CommandsHandler
{
    private Value[] _commandArguments;
    private readonly Dictionary<string, Func<Value>> _commands;
    private Dictionary<string, string> _sets;
    private Dictionary<int, Dictionary<string, string>> _hsets;

    public CommandsHandler()
    {
        _commandArguments = new Value[] { new() { Type = nameof(DataTypes.NULL) } };
        _commands = new() { 
            { "PING", () => Ping(_commandArguments) },
            { "SET", () => Set(_commandArguments) },
            { "GET", () => Get(_commandArguments) },
            { "HSET", () => HSet(_commandArguments) },
            { "HGET", () => HGet(_commandArguments) },
            { "COMMAND", () => new Value() { Type = nameof(DataTypes.STRING), String = "OK" } }
        };
        _sets = new();
        _hsets = new();
    }

    public Value HandleCommand(Value commandArguments)
    {
        if(commandArguments is null || commandArguments?.Array is null)
            return new Value() { Type = nameof(DataTypes.ERROR), Error = "ERR theres no arguments" };

        _commandArguments = commandArguments.Array[1..];
        
        return !_commands.TryGetValue(commandArguments.Array[0].Bulk.ToUpper(), out var result)
        ? new Value() { Type = nameof(DataTypes.ERROR), Error = "ERR invalid command" }
        : result();
    }
    
    private Value Ping(Value[] commandArguments)
    {
        return commandArguments.Length > 0 
            ? new Value() { Type = nameof(DataTypes.STRING), String = commandArguments[0].Bulk } 
            : new Value() { Type = nameof(DataTypes.STRING), String = "PONG"};
    }

    private Value Set(Value[] commandArguments)
    {
        if (commandArguments.Length != 2)
            return new Value() { Type = nameof(DataTypes.ERROR), Error = "ERR wrong number of arguments for 'set' command" };

        string key = commandArguments[0].Bulk;
        string value = commandArguments[1].Bulk;

        if (!_sets.TryAdd(key, value))
            _sets[key] = value;
            
        return new Value() { Type = nameof(DataTypes.STRING), String = "OK" };
    }

    private Value Get(Value[] commandArguments)
    {
        if (commandArguments.Length != 1)
            return new Value() { Type = nameof(DataTypes.ERROR), Error = "ERR wrong number of arguments for 'get' command" };

        _sets.TryGetValue(commandArguments[0].Bulk, out string? result);
        
        return result is null 
        ? new Value() { Type = nameof(DataTypes.NULL) } 
        : new Value() { Type = nameof(DataTypes.BULK), Bulk = result };
    }
 
    private Value HSet(Value[] commandArguments)
    {
        if(commandArguments.Length != 3)
            return new Value() { Type = nameof(DataTypes.ERROR), Error = "ERR wrong number of arguments for 'hset' command" };

        int hash = commandArguments[0].Bulk.GetHashCode();
        string key = commandArguments[1].Bulk;
        string value = commandArguments[2].Bulk;

        if(!_hsets.TryAdd(hash, new() {{key, value}}))
        {
            if(!_hsets[hash].TryAdd(key, value))
                _hsets[hash][key] = value;
        }

        return new Value() { Type = nameof(DataTypes.STRING), String = "OK" };
    }

    private Value HGet(Value[] commandArguments)
    {
        if (commandArguments.Length != 2)
            return new Value() { Type = nameof(DataTypes.ERROR), Error = "ERR wrong number of arguments for 'hget' command" };

        int hash = commandArguments[0].Bulk.GetHashCode();
        string key = commandArguments[1].Bulk;

        string? result = null;

        _hsets.TryGetValue(hash, out var set);

        if(set != null && set.TryGetValue(key, out var value))
            result = value;
    
        return result is null
        ? new Value(){Type = nameof(DataTypes.NULL)} 
        : new Value() { Type = nameof(DataTypes.BULK), Bulk =  result};
    }
}