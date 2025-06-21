using Sandbox;

public sealed class Villager : EnemyBase
{
    public NavMeshAgent Agent { get; set; }
    private TimeUntil _delay;

    protected override void OnStart()
    {
        GameObject.Name = $"😈 Enemy - {GameObject.Name}";

        Agent = GetComponent<NavMeshAgent>();

        _delay = 0f;
    }

	protected override void OnUpdate()
	{
        if (!_delay) return;

        var startPos = WorldPosition;
        var point = Scene.NavMesh.GetClosestEdge(startPos, 8000f);

        //Agent.Velocity *= 50f;

        Log.Info(point.Value);

        if (point.HasValue) // cuz GetClosesPoint give Vector3? (nullable) instead of Vector3
            Agent.MoveTo(point.Value);

        Gizmo.Draw.Color = Color.Orange;
        Gizmo.Draw.LineThickness = 5;
        Gizmo.Draw.Arrow(startPos, point.Value);

        _delay = 1f;
    }

    public override void OnTriggerEnter(Collider other)
    {
        
    }

    public override void OnDamage(in DamageInfo damage)
    {

    }
}
