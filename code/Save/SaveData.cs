using Sandbox;

public sealed class SaveData
{
    public Transform PlayerTransform { get; set; }

    public void Push()
    {
        var ply = Player.Instance;

        PlayerTransform = ply.WorldTransform;
    }

    public void Pull()
    {
        var ply = Player.Instance;

        ply.WorldTransform = PlayerTransform;
    }
}