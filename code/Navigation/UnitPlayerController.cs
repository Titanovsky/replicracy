using Sandbox;
using System.ComponentModel.DataAnnotations;

public sealed class UnitPlayerController : Component
{
    [Property] public Player Player { get; set; } = null;

    [Property]
    [Required]
    private NavMeshAgent Agent { get; set; } = null;

    protected override void OnStart()
    {
        Agent = Components.Get<NavMeshAgent>();

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
        Agent.MoveTo(position);
        Log.Info($"Specified position: {position}");
    }
}
