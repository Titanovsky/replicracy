using Sandbox;

public sealed class Cultist : EnemyBase
{
    [Property] public EmotionsController EmotionsController { get; set; }
    [Property] public SkinnedModelRenderer Renderer { get; set; }
    [Property] public NavMeshAgent NavMeshAgent { get; set; }
    [Property] public float SearchRadius { get; set; } = 500f;
    [Property] public float LostRadius { get; set; } = 700f;
    [Property] public float MovingStartPosRadius { get; set; } = 150f;
    [Property] public float MoveDelay { get; set; } = 10f;

    [Property][Category("Weapon")] public GameObject AttackPosition { get; set; }
    [Property][Category("Weapon")] public float AttackDamage { get; set; } = 6f;
    [Property][Category("Weapon")] public float AttackDelay { get; set; } = 1f;
    [Property][Category("Weapon")] public float AttackDistance { get; set; } = 30f;

    private CultistState CurrentState { get; set; }

    private Vector3 _spawnPosition;
    private Vector3 _randomPointMoving;

    private RealTimeUntil _delayMovingTimer;
    private RealTimeUntil _delayAttackTimer;

    private GameObject _attackTarget;
    private GameObject _lastAttacker;

    private Sphere searchSphere;
    private SceneTraceResult tr;

    public enum CultistState
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
        FollowToTarger();

        Moving();

        CheckDistanceToTarget();
        SearchTarget();
    }

    private void Moving()
    {
        if (CurrentState != CultistState.Idle) return;

        if (!_delayMovingTimer) return;

        if (Scene.NavMesh.GetRandomPoint(_spawnPosition, MovingStartPosRadius).HasValue)
        {
            _randomPointMoving = Scene.NavMesh.GetRandomPoint(_spawnPosition, MovingStartPosRadius).Value;

            NavMeshAgent.MoveTo(_randomPointMoving);
        }

        ResetMovingTimer();
    }

    private void SearchTarget()
    {
        if (CurrentState != CultistState.Idle) return;

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
        if (CurrentState != CultistState.Idle) return;

        Vector3 direction = (_randomPointMoving - WorldPosition).Normal;

        RotateTo(direction);
    }

    private void Attack()
    {
        if (CurrentState != CultistState.Attack) return;

        if (!_delayAttackTimer) return;

        var origin = AttackPosition.WorldPosition;
        Vector3 direction = (_attackTarget.WorldPosition - AttackPosition.WorldPosition).Normal;
        var directionRotate = Rotation.LookAt(new Vector3(direction.x, direction.y, 0)) * Vector3.Forward;

        tr = Scene.Trace.Ray(new Ray(origin, directionRotate), AttackDistance)
            .IgnoreGameObject(GameObject)
            .Run();

        if (!tr.Hit)
            return;

        var hitObject = tr.GameObject;

        if (hitObject == _attackTarget)
        {
            var damagable = hitObject.Parent.GetComponentInChildren<IDamageable>();

            if (damagable is not null)
            {
                damagable.OnDamage(new(10, GameObject, GameObject));
            }
        }

        ResetAttackTimer();
    }

    private void FollowToTarger()
    {
        if (CurrentState != CultistState.Attack) return;

        var distanceForAttack = AttackDistance / 2;
        var postion = _attackTarget.WorldPosition - new Vector3(distanceForAttack, distanceForAttack, 0);

        NavMeshAgent.MoveTo(postion);
    }

    private void CheckDistanceToTarget()
    {
        if (CurrentState != CultistState.Attack) return;

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
        if (_attackTarget == null) return;

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
        Log.Info($"[Cultist] Die from {_lastAttacker}");

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

        if (target.Tags.Has("villager"))
            return false;

        if (target.Tags.Has("allien"))
            return false;

        return base.IsFriend(target);
    }

    public void SetIdleState()
    {
        CurrentState = CultistState.Idle;
        EmotionsController.SetEmotion(EmotionsController.Emotions.Idle);
    }

    public void SetAttackState()
    {
        CurrentState = CultistState.Attack;
        EmotionsController.SetEmotion(EmotionsController.Emotions.Angry);
    }

    public void SetTarget(GameObject target)
    {
        _attackTarget = target;

        SetAttackState();
    }

    private void ResetMovingTimer() => _delayMovingTimer = MoveDelay;
    private void ResetAttackTimer() => _delayAttackTimer = AttackDelay;
}
