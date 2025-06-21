using Sandbox;

public sealed class Villager : EnemyBase
{
    public NavMeshAgent Agent { get; set; }
    private TimeUntil _delay;

    private Vector3 _up = new Vector3(0, 0, 50f);
    private Vector3 _targetPos;

    private void Prepare()
    {
        GameObject.Name = $"😈 Enemy - {GameObject.Name}";

        Agent = GetComponent<NavMeshAgent>();

        _delay = 0f;
    }

    private void Move()
    {
        var startPos = WorldPosition;

        if (!_targetPos.IsNaN)
        {
            Gizmo.Draw.Color = Color.Orange;
            Gizmo.Draw.LineThickness = 4;
            Gizmo.Draw.Arrow(startPos + _up, _targetPos);
        }

        if (!_delay) return;

        var point = Scene.NavMesh.GetRandomPoint(startPos, 256f);

        //Agent.Velocity *= 2f;

        if (point.HasValue) // cuz GetClosesPoint give Vector3? (nullable) instead of Vector3
        {
            Agent.MoveTo(point.Value);

            _targetPos = point.Value;
        }

        _delay = .75f;
    }

    protected override void OnStart()
    {
        Prepare();
    }

	protected override void OnUpdate()
	{
        Move();
    }

    public override void OnDamage(in DamageInfo damage)
    {
        Log.Info($"Hit ");
    }
}
