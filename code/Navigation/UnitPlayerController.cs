using Sandbox;

public sealed class UnitPlayerController : Component
{
    [Property] public Player Player { get; set; } = null;

    protected override void OnStart()
    {
        Player = Player.Instance;

        Player.OnSpecified += Test;
    }

    protected override void OnDestroy()
    {
        Player.OnSpecified -= Test;

        Player = null;
    }

    private void Test(Vector3 position)
    {
        Log.Info($"Specified position: {position}");
    }
}
