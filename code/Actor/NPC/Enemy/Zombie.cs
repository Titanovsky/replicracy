using Sandbox;

public sealed class Zombie : EnemyBase
{
    [Property] public EmotionsController EmotionsController { get; set; }
    [Property] public SkinnedModelRenderer Renderer { get; set; }
    [Property] public NavMeshAgent NavMeshAgent { get; set; }
    [Property] public float SearchRadius { get; set; } = 500f;
    [Property] public float LostRadius { get; set; } = 700f;
    [Property] public float MovingRadius { get; set; } = 150f;
    [Property] public float MoveDelay { get; set; } = 10f;

    [Property][Category("Weapon")] public float AttackDamage { get; set; } = 3f;
    [Property][Category("Weapon")] public float AttackDelay { get; set; } = 1f;

    private ZombieState CurrentState { get; set; }

    private Vector3 _spawnPosition;
    private Vector3 _randomPointMoving;

    private RealTimeUntil _delayMovingTimer;
    private RealTimeUntil _delayAttackTimer;

    private GameObject _attackTarget;
    private GameObject _lastAttacker;

    private Sphere searchSphere;


    public enum ZombieState
    {
        Idle,
        Attack
    }

    protected override void OnStart()
    {
        _spawnPosition = WorldPosition;

        SetIdleState();
    }

    protected override void OnUpdate()
    {
        RotateToTarget();
        RotateToMovingPoint();

        Attack();
        Moving();

        CheckDistanceToTarget();
        SearchTarget();
    }

    private void Moving()
    {
        if (!_delayMovingTimer) return;

        _randomPointMoving = (Vector3)Scene.NavMesh.GetRandomPoint(_spawnPosition, MovingRadius);

        NavMeshAgent.MoveTo(_randomPointMoving);

        ResetMovingTimer();
    }

    private void SearchTarget()
    {
        if (CurrentState != ZombieState.Idle) return;

        searchSphere = new Sphere(WorldPosition, SearchRadius);

        var objectInSphere = Scene.FindInPhysics(searchSphere);

        foreach (var item in objectInSphere)
        {
            if (!IsFriend(item))
                SetTarget(item);
        }
    }

    private void RotateToMovingPoint()
    {
        if (CurrentState != ZombieState.Idle) return;

        Vector3 direction = (_randomPointMoving - WorldPosition).Normal;

        RotateTo(direction);
    }

    private void Attack()
    {
        if (CurrentState != ZombieState.Attack) return;

        //if (!_delayReloadTimer) return;

        
    }

    private void CheckDistanceToTarget()
    {
        if (CurrentState != ZombieState.Attack) return;

        if (_attackTarget == null || !_attackTarget.IsValid())
        {
            SetIdleState();
            return;
        }

        var distance = WorldPosition.Distance(_attackTarget.WorldPosition);

        if (distance > LostRadius)
        {
            SetIdleState();
        }
    }

    private void RotateToTarget()
    {
        //if (CurrentState != PoliceState.Attack) return;

        Vector3 direction = (_attackTarget.WorldPosition - WorldPosition).Normal;

        RotateTo(direction);
    }

    private void RotateTo(Vector3 direction)
    {
        Rotation rotate = Rotation.LookAt(new Vector3(direction.x, direction.y, 0));

        WorldRotation = Rotation.Lerp(WorldRotation, rotate, 5 * Time.Delta);
    }

    public override void OnDamage(in DamageInfo dmgInfo)
    {
        Health -= dmgInfo.Damage;
        _lastAttacker = dmgInfo.Attacker;

        if (Health <= 0)
            Die();

        _delayMovingTimer = 0;
    }

    public override void Die()
    {
        Log.Info($"[Police] Die from {_lastAttacker}");

        if (_lastAttacker == Player.Instance.GameObject)
        {
            var ply = Player.Instance;

            ply.Frags += 1;
            ply.HeaderLevel.Show();
        }

        DestroyGameObject();
    }

    public override bool IsFriend(GameObject target)
    {
        if (target.Tags.Has("player"))
            return false;

        if (target.Tags.Has("replicant"))
            return false;

        if (target.Tags.Has("undead"))
            return false;

        if (target.Tags.Has("allien"))
            return false;

        return base.IsFriend(target);
    }

    public void SetIdleState()
    {
        CurrentState = ZombieState.Idle;
        EmotionsController.SetEmotion(EmotionsController.Emotions.Idle);

        MoveDelay = MoveDelay * 2f;
    }

    public void SetAttackState()
    {
        CurrentState = ZombieState.Attack;
        EmotionsController.SetEmotion(EmotionsController.Emotions.Angry);

        MoveDelay = MoveDelay / 2f;
    }

    public void SetTarget(GameObject target)
    {
        _attackTarget = target;

        SetAttackState();
    }

    private void ResetMovingTimer() => _delayMovingTimer = MoveDelay;
    private void ResetAttackTimer() => _delayAttackTimer = AttackDelay;
}
