using Sandbox;
using System.ComponentModel.DataAnnotations;

public sealed class UnitPlayerController : Component
{
    [Property] public Player Player { get; set; }
    [Property][Required] private NavMeshAgent Agent { get; set; }

    protected override void OnStart()
    {
        Agent = Components.Get<NavMeshAgent>();

        Player = Player.Instance;

        Subribe();
    }

    protected override void OnDestroy()
    {
        Unsubscribe();

        Player = null;
    }

    private void Subribe()
    {
        Player.OnSpecified += MoveToPlayerSpecify;
    }

    private void Unsubscribe()
    {
        Player.OnSpecified -= MoveToPlayerSpecify;
    }

    private void MoveToPlayerSpecify(Vector3 position)
    {
        Agent.MoveTo(position);
    }
}
