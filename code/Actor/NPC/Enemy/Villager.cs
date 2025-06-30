using Sandbox;

public sealed class Villager : EnemyBase
{
    [Property] public NavMeshAgent Agent { get; set; }
    [Property] public SkinnedModelRenderer Renderer { get; set; }
    [Property] public EmotionsController EmotionsController { get; set; }
    [Property] public int DNA { get; set; } = 1;
    [Property, Category("Stats")] public override float Health { get; set; } = 10f; 

    private TimeUntil _delayMoving;

    private Vector3 _up = new Vector3(0, 0, 50f);
    private Vector3 _targetPos;
    private float _animWorkaround = .5f; //! fuck anim move_x and move_y

    private Color32 _white = Color.White;
    private Color32 _red = Color.Red;
    private TimeUntil _delayBlockDamage;

    private GameObject _lastAttacker;

    public override void Die()
    {
        if (_lastAttacker == Player.Instance.GameObject || _lastAttacker.Tags.Has("replicant"))
        {
            var ply = Player.Instance;

            ply.Frags += 1;
            ply.Dna += DNA;
            ply.HeaderLevel.Show();
        }

        DestroyGameObject();
    }

    private void Prepare()
    {
        GameObject.Name = $"😈 Enemy - {GameObject.Name}";

        Agent = GetComponent<NavMeshAgent>();

        _delayMoving = 0f;

        //Player.Instance.PlayerController.
    }

    private void PlayAnimation()
    {
        var dir = Agent.Velocity;
        var forward = WorldRotation.Forward.Dot(dir);
        var sideward = WorldRotation.Right.Dot(dir);

        //todo aim_head
        //Renderer.Set("aim_head", (forward - _targetPos).Normal);
        Renderer.Set("move_y", sideward * _animWorkaround);
        Renderer.Set("move_x", forward * _animWorkaround);
    }

    private void Move()
    {
        PlayAnimation();

        var startPos = WorldPosition;
        
        if (GlobalSettings.IsDebug && !_targetPos.IsNaN)
        {
            Gizmo.Draw.Color = Color.Orange;
            Gizmo.Draw.LineThickness = 4;
            Gizmo.Draw.Arrow(startPos + _up, _targetPos);
        }

        if (!_delayMoving) return;

        var point = Scene.NavMesh.GetRandomPoint(startPos, 256f);

        //Agent.Velocity *= 2f;

        if (point.HasValue) // cuz GetClosesPoint give Vector3? (nullable) instead of Vector3
        {
            Agent.MoveTo(point.Value);

            _targetPos = point.Value;
        }

        _delayMoving = 1f;
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
        _lastAttacker = dmgInfo.Attacker;

        if (Health <= 0)
            Die();
    }

    public override bool IsFriend(GameObject target)
    {
        if (target.Tags.Has("villager"))
            return true;

        return base.IsFriend(target);
    }
}
