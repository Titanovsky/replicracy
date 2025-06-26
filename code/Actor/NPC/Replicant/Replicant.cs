using Replicracy.Common;

public sealed class Replicant : Component, Component.IDamageable
{
    private static readonly Logger Log = new("Replicant");

    [Property][Category("Movement")] public float RotationSpeed { get;  set; } = 2.5f;
    [Property][Category("Movement")] public float MovementSpeed { get; set; } = 140;
    [Property][Category("Attack")] public float AttackDelay { get; set; } = 1f;
    [Property][Category("Attack")] public int AttackDamage { get; set; } = 5;
    [Property][Category("Health")] public float Health { get; set; } = 20;
    [Property][Category("Health")] public float MaxHealth { get; set; } = 20;
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

    public void TakeDamage(in DamageInfo dmgInfo)
    {
        var damage = dmgInfo.Damage;

        Health -= damage;

        Log.Info($"{GameObject} take damage {damage}f by {dmgInfo.Attacker}");

        if (Health <= 0)
            Die();
    }

    public void Die()
    {
        //todo sound.replicant.die
        //todo particle

        var repController = Player.Instance.ReplicantController;
        var hasLive = repController.Replicants.Contains(this);
        if (hasLive)
            repController.RemoveReplicant(this);

        Log.Info($"{GameObject} died");

        DestroyGameObject();
    }

    public void SetTargetPoint(Vector3 point) => _targerPoint = point;
    public void MoveToPoint(Vector3 point) => NavMeshAgent.MoveTo(point);

    public float GetRadius() => NavMeshAgent.Radius;
    public GameObject GetEye() => eye;
    public Vector3 GetTargetPoint() => _targerPoint;
    public void ReseAttackTimer() => _attackTimer = AttackDelay;
    public bool IsAttackAllowed() => _attackTimer;

    public void OnDamage(in DamageInfo dmgInfo)
    {
        TakeDamage(dmgInfo);
    }
}
