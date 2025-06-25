public sealed class Replicant : Component
{
    [Property][Category("Movement")] public float RotationSpeed { get; private set; } = 2.5f;
    [Property][Category("Movement")] public float MovementSpeed { get; private set; } = 140;
    [Property][Category("Attack")] public float AttackDelay { get; private set; } = 1f;
    [Property][Category("Attack")] public int AttackDamage { get; private set; } = 5;
    [Property][Category("Health")] public float Health { get; private set; } = 100;
    [Property][Category("Health")] public float MaxHealth { get; private set; } = 100;
    [Property][Category("Other")] GameObject eye { get; set; }

    [RequireComponent] NavMeshAgent NavMeshAgent { get; set; }

    private Vector3 _targerPoint;

    private RealTimeUntil _attackTimer;
    public ReplicantFSM replicantFSM;

    protected override void OnAwake()
    {
        replicantFSM = new();
        _attackTimer = AttackDelay;

        replicantFSM.AddState(new ReturnToPlayer(this));
        replicantFSM.AddState(new MoveToPoint(this));
        replicantFSM.AddState(new AttackBuilding(this));
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

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            GameObject.Destroy();
        }
    }

    public void SetTargetPoint(Vector3 point) => _targerPoint = point;
    public void MoveToPoint(Vector3 point) => NavMeshAgent.MoveTo(point);

    public float GetRadius() => NavMeshAgent.Radius;
    public GameObject GetEye() => eye;
    public Vector3 GetTargetPoint() => _targerPoint;
    public void ReseAttackTimer() => _attackTimer = AttackDelay;
    public bool IsAttackAllowed() => _attackTimer;
}
