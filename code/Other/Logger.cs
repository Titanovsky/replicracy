namespace Replicracy.Common;

public class Logger
{
    private string _Name { get; set; }

    public Logger(string name)
    {
        _Name = name;
    }

    public void Info(object msg)
    {
        Log.Info($"[{_Name}] {msg}");
    }

    public void Warning(object msg)
    {
        Log.Warning($"[{_Name}] {msg}");
    }

    public void Error(object msg)
    {
        Log.Error($"[{_Name}] {msg}");
    }
}