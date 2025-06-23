using Sandbox;

public sealed class Villager : EnemyBase
{
    [Property] public NavMeshAgent Agent { get; set; }
    [Property] public ModelRenderer Renderer { get; set; }
    [Property, Category("Stats")] public override float Health { get; set; } = 10f; 

    private TimeUntil _delay;

    private Vector3 _up = new Vector3(0, 0, 50f);
    private Vector3 _targetPos;

    private Color32 _white = Color.White;
    private Color32 _red = Color.Red;
    private TimeUntil _delayBlockDamage;

    private void Prepare()
    {
        GameObject.Name = $"😈 Enemy - {GameObject.Name}";

        Agent = GetComponent<NavMeshAgent>();

        _delay = 0f;

        //Player.Instance.PlayerController.
    }

    private void Move()
    {
        var startPos = WorldPosition;

        if (GlobalSettings.IsDebug && !_targetPos.IsNaN)
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

        _delay = 1f;
    }

    private void ResetColor()
    {
        if (!_delayBlockDamage) return;

        Renderer.Tint = _white;
    }

    protected override void OnStart()
    {
        Prepare();
    }

	protected override void OnUpdate()
	{
        Move();
        ResetColor();
    }

    public override void OnDamage(in DamageInfo dmgInfo)
    {
        Health -= dmgInfo.Damage;
        Renderer.Tint = _red;
        _delayBlockDamage = 1f;

        if (Health <= 0)
            DestroyGameObject();
    }

    public override bool IsFriend(GameObject target)
    {
        if (target.Tags.Has("villager"))
            return true;

        return base.IsFriend(target);
    }
}
