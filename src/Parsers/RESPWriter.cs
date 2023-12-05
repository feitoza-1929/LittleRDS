using System.Text;

public class RESPWriter : IRESPWriter
{
    public byte[] Init(Value value)
    {
        return Write(value);
    }
    private byte[] Write(Value value)
    {
        switch (value.Type)
        {
            case nameof(DataTypes.ARRAY):
                return WriteArray(value);
            case nameof(DataTypes.BULK):
                return WriteBulk(value);
            case nameof(DataTypes.STRING):
                return WriteString(value);
            case nameof(DataTypes.NULL):
                return WriteNull();
            case nameof(DataTypes.ERROR):
                return WriteError(value);
            default:
                return Array.Empty<byte>();
        }
    }

    private byte[] WriteError(Value value)
    {
        return Encoding.ASCII.GetBytes($"{DataTypes.ERROR}{value.Error}\r\n");
    }

    private byte[] WriteNull()
    {
        return Encoding.ASCII.GetBytes($"{DataTypes.NULL}\r\n");
    }

    private byte[] WriteString(Value value)
    {
        return Encoding.ASCII.GetBytes($"{DataTypes.STRING}{value.String}\r\n");
    }

    private byte[] WriteBulk(Value value)
    {
        return Encoding.ASCII.GetBytes($"{DataTypes.BULK}{value.Bulk.Length}\r\n{value.Bulk}\r\n");
    }

    private byte[] WriteArray(Value value)
    {
        byte[] result = Encoding.ASCII.GetBytes($"{DataTypes.ARRAY}{value.Array.Count()}\r\n");
        for(int i = 0; i < value.Array.Length; i++)
        {
            result.Union(Write(value.Array[i]));
        }

        return result;
    }
}