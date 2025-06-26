namespace Replicracy.Common;

public class Logger
{
    public string Name { get; set; }

    public Logger(string name)
    {
        Name = name;
    }

    public void Info(object msg)
    {
        Log.Info($"[{Name}] {msg}");
    }

    public void Warning(object msg)
    {
        Log.Warning($"[{Name}] {msg}");
    }

    public void Error(object msg)
    {
        Log.Error($"[{Name}] {msg}");
    }
}