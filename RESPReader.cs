public class RESPReader
{
    public Value Init(string sourceClientCommand)
    {
        using StringReader reader = new(sourceClientCommand);
        return Read(reader);
    }
    private Value Read(StringReader reader)
    {
        char dataType = (char)reader.Read();

        switch (dataType)
        {
            case DataTypes.ARRAY:
                return ReadArray(reader);
            case DataTypes.BULK:
                return ReadBulk(reader);
            default:
                Console.WriteLine($"Unknown type: {dataType}");
                return new Value();
        }
    }

    private Value ReadBulk(StringReader reader)
    {
        reader.ReadLine();
        return new Value() { Type = nameof(DataTypes.BULK), Bulk = reader.ReadLine() };
    }


    private Value ReadArray(StringReader reader)
    {
        int? arraySize = ReadInteger(reader);
        List<Value> values = new();

        for (int i = 0; i < arraySize; i++)
        {
            Value value = Read(reader);
            values.Add(value);
        }

        return new() { Type = nameof(DataTypes.ARRAY), Array = values.ToArray() };
    }

    private int ReadInteger(StringReader reader) 
        => int.Parse(reader?.ReadLine());
}