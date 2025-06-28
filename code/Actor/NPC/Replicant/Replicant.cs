using Replicracy.Common;

public sealed class Replicant : Component, Component.IDamageable
{
    private static readonly Logger Log = new("Replicant");

    [Property][Category("Movement")] public float RotationSpeed { get; set; } = 2.5f;
    [Property][Category("Movement")] public float MovementSpeed { get; set; } = 140;
    [Property][Category("Movement")] public float MaxDistanceToPlayer { get; set; } = 2000;
    [Property][Category("Attack")] public float AttackDelay { get; set; } = 1f;
    [Property][Category("Attack")] public int AttackDamage { get; set; } = 5;
    [Property][Category("Health")] public float AttackDistance { get; set; } = 40f;
    [Property][Category("Health")] public float Health { get; set; } = 20;
    [Property][Category("Health")] public float MaxHealth { get; set; } = 20;
    [Property][Category("Other")] GameObject eye { get; set; }
    [Property][Category("Other")] public SkinnedModelRenderer Renderer { get; set; }
    [Property][Category("Other")] public ReplicantHealthBar HealthBar { get; set; }

    [RequireComponent] public NavMeshAgent Agent { get; set; }

    private Vector3 _targerPoint;
    private GameObject _targerObject;

    private RealTimeUntil _attackTimer;
    public ReplicantFSM replicantFSM;

    private SkinnedModelRenderer _modelRenderer;
    private Collider _collder;

    protected override void OnAwake()
    {
        replicantFSM = new();
        _attackTimer = AttackDelay;

        _modelRenderer = GameObject.GetComponentInChildren<SkinnedModelRenderer>();
        _collder = GameObject.GetComponent<Collider>();

        replicantFSM.AddState(new ReturnToPlayer(this));
        replicantFSM.AddState(new MoveToPoint(this));

        replicantFSM.AddState(new AttackBuilding(this));

        replicantFSM.AddState(new FollowToEnemy(this));
        replicantFSM.AddState(new HandleAttackEnemy(this));

        replicantFSM.AddState(new Idle(this));
    }

    protected override void OnStart()
    {
        replicantFSM.SetState<ReturnToPlayer>();
    }

    protected override void OnUpdate()
    {
        replicantFSM.CurrentState?.Update();
    }

    [Property] public SoundEvent TakeDamageSound;
    public void TakeDamage(in DamageInfo dmgInfo)
    {
        var damage = dmgInfo.Damage;

        Health -= damage;
        Sound.Play(TakeDamageSound, WorldPosition);

        Log.Info($"{GameObject} take damage {damage}f by {dmgInfo.Attacker}");

        if (Health <= 0)
            Die();
    }

    public void Die()
    {
        var repController = Player.Instance.ReplicantController;
        var hasLive = repController.Replicants.Contains(this);
        if (hasLive)
            repController.RemoveReplicant(this);

        Log.Info($"{GameObject} died");

        DestroyGameObject();
    }

    public void SetAttackEnemy(GameObject targetObject)
    {
        SetTargetObject(targetObject);
        replicantFSM.SetState<FollowToEnemy>();
    }

    public void SetAttackBuilding(Vector3 targetPosition)
    {
        SetTargetPoint(targetPosition);
        replicantFSM.SetState<AttackBuilding>();
    }

    public void SetMoveToPoint(Vector3 targetPosition)
    {
        SetTargetPoint(targetPosition);
        replicantFSM.SetState<MoveToPoint>();
    }

    public void SetTargetPoint(Vector3 point) => _targerPoint = point;
    public void SetTargetObject(GameObject targerObject) => _targerObject = targerObject;
    public void MoveToPoint(Vector3 point) => Agent.MoveTo(point);

    public float GetRadius() => Agent.Radius;
    public GameObject GetEye() => eye;
    public Vector3 GetTargetPoint() => _targerPoint;
    public GameObject GetTargetObject() => _targerObject;
    public void ReseAttackTimer() => _attackTimer = AttackDelay;
    public bool IsAttackAllowed() => _attackTimer;

    public void HideReplicant()
    {
        Agent.Enabled = false;
        _modelRenderer.Enabled = false;
        _collder.IsTrigger = true;
    }

    public void ShowReplicant()
    {
        Agent.Enabled = true;
        _modelRenderer.Enabled = true;
        _collder.IsTrigger = false;
    }

    public void OnDamage(in DamageInfo dmgInfo)
    {
        TakeDamage(dmgInfo);
    }
}
