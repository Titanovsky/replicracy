using Sandbox;

public sealed class Cultist : EnemyBase
{
    [Property] public SkinnedModelRenderer Renderer { get; set; }
    [Property] public NavMeshAgent NavMeshAgent { get; set; }
    [Property] public float SearchRadius { get; set; } = 500f;
    [Property] public float LostRadius { get; set; } = 700f;
    [Property] public float MovingStartPosRadius { get; set; } = 150f;
    [Property] public float MoveDelay { get; set; } = 10f;
    [Property] public int DNA { get; set; } = 1;
    [Property] public SoundEvent TakeDamageSound { get; set; }

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

    private Sphere _searchSphere;
    private SceneTraceResult _tr;

    private float _delayBlockDamage = 0.3f;
    private Color32 _white = Color.White;
    private Color32 _red = Color.Red;
    private TimeUntil _delayBlockDamageTimer;

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

        ResetColor();
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

        _searchSphere = new Sphere(WorldPosition, SearchRadius);

        var objectInSphere = Scene.FindInPhysics(_searchSphere);

        foreach (var item in objectInSphere)
        {
            if (!IsFriend(item))
                if (IsTargetVisible(item, SearchRadius))
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

        if (!IsTargetVisible(_attackTarget, LostRadius)) return;

        var origin = AttackPosition.WorldPosition;
        Vector3 direction = (_attackTarget.WorldPosition - AttackPosition.WorldPosition).Normal;
        var directionRotate = Rotation.LookAt(new Vector3(direction.x, direction.y, 0)) * Vector3.Forward;

        _tr = Scene.Trace.Ray(new Ray(origin, directionRotate), AttackDistance)
            .IgnoreGameObject(GameObject)
            .Run();

        if (_tr.Hit)
        {
            var hitObject = _tr.GameObject;

            var damagable = hitObject.Parent.GetComponentInChildren<IDamageable>();

            if (damagable is not null)
            {
                damagable.OnDamage(new(AttackDamage, GameObject, GameObject));
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

        Renderer.Tint = _red;
        ResetBlockDamageTimer();

        Sound.Play(TakeDamageSound, WorldPosition);

        SetTarget(dmgInfo.Attacker);

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
            ply.Dna += DNA;
            ply.HeaderLevel.Show();
        }

        DestroyGameObject();
    }

    private void ResetColor()
    {
        if (!_delayBlockDamageTimer) return;

        Renderer.Tint = _white;
    }

    private bool IsTargetVisible(GameObject target, float visibleRadius)
    {
        if (!target.IsValid()) return false;

        var origin = AttackPosition.WorldPosition;
        var upTargetPosition = target.WorldPosition.WithZ(30f);
        Vector3 direction = (upTargetPosition - AttackPosition.WorldPosition).Normal;
        var directionRotate = Rotation.LookAt(new Vector3(direction.x, direction.y, direction.z)) * Vector3.Forward;

        _tr = Scene.Trace.Ray(new Ray(origin, directionRotate), visibleRadius)
            .IgnoreGameObject(GameObject)
            .Run();

        if (_tr.GameObject == target || !IsFriend(_tr.GameObject))
            return true;

        return false;
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
    }

    public void SetAttackState()
    {
        CurrentState = CultistState.Attack;
    }

    public void SetTarget(GameObject target)
    {
        _attackTarget = target;

        SetAttackState();
    }

    private void ResetMovingTimer() => _delayMovingTimer = MoveDelay;
    private void ResetAttackTimer() => _delayAttackTimer = AttackDelay;
    private void ResetBlockDamageTimer() => _delayBlockDamageTimer = _delayBlockDamage;
}
