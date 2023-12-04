using System.Text;

public static class FileIO
{
    public static void Write(string path, string content)
    {
        using StreamWriter fs = File.AppendText(path);
        fs.Write(content);
    }

    public static string? Read(string path)
    {
        if (!File.Exists(path))
            return null;

        string result;
        using StreamReader sr = File.OpenText(path);
        while ((result = sr.ReadToEnd()) != null)
        {
            return result;
        }
        return result;
    }
}