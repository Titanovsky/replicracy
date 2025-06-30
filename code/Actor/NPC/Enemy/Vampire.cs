using Sandbox;
using System;
using static Sandbox.PhysicsContact;

public sealed class Vampire : EnemyBase
{
    [Property] public SkinnedModelRenderer Renderer { get; set; }
    [Property] public float SearchRadius { get; set; } = 500f;
    [Property] public float LostRadius { get; set; } = 700f;
    [Property] public float MovingDelay { get; set; } = 5f;
    [Property] public float MovingRadius { get; set; } = 150f;
    [Property] public float AttackDamage { get; set; } = 10f;
    [Property] public float AttackDelay { get; set; } = 6f;
    [Property] public int DNA { get; set; } = 1;
    [Property] public SoundEvent AttackSound { get; set; }
    [Property] public SoundEvent TakeDamageSound { get; set; }
    [Property] public GameObject AttackPosition { get; set; }
    [Property] public GameObject ProjectilePrefab { get; set; }

    private VampireState CurrentState { get; set; }

    private Vector3 _spawnPosition;
    private RealTimeUntil _delayMovingTimer;
    private RealTimeUntil _delayAttackTimer;

    [Property] private GameObject _attackTarget;
    private GameObject _lastAttacker;

    private Sphere _searchSphere;
    private SceneTraceResult _tr;

    private float _delayBlockDamage = 0.3f;
    private Color32 _white = Color.White;
    private Color32 _red = Color.Red;
    private TimeUntil _delayBlockDamageTimer;

    public enum VampireState
    {
        Idle,
        Attack
    }

    protected override void OnStart()
    {
        _spawnPosition = WorldPosition;

        CurrentState = VampireState.Idle;
    }

    protected override void OnUpdate()
    {
        Moving();
        CheckDistanceToTarget();

        SearchTarget();

        RotateToTarget();
        Attack();

        ResetColor();
    }

    private void Moving()
    {
        if (!_delayMovingTimer) return;

        var randomPoint = GetRandomPoint();
        bool isAllowed = IsMovingAllowed(randomPoint);

        if (!isAllowed) return;

        WorldPosition = randomPoint;
        RandomRotate();

        ResetMovingTimer();
    }

    private void CheckDistanceToTarget()
    {
        if (CurrentState != VampireState.Attack) return;

        if (_attackTarget == null || !_attackTarget.IsValid())
        {
            CurrentState = VampireState.Idle;
            return;
        }

        var distance = WorldPosition.Distance(_attackTarget.WorldPosition);

        if (distance > LostRadius)
        {
            _attackTarget = null;

            CurrentState = VampireState.Idle;
        }
    }

    private void RotateToTarget()
    {
        if (CurrentState != VampireState.Attack) return;

        Vector3 direction = (_attackTarget.WorldPosition - WorldPosition).Normal;

        Rotation rotate = Rotation.LookAt(new Vector3(direction.x, direction.y, 0));

        WorldRotation = Rotation.Lerp(WorldRotation, rotate, 5 * Time.Delta);
    }

    private void Attack()
    {
        if (CurrentState != VampireState.Attack) return;

        if (!_delayAttackTimer) return;
        ResetAttackTimer();

        if (!IsTargetVisible(_attackTarget, LostRadius)) return;

        var origin = AttackPosition.WorldPosition;
        var upAttackPosition = _attackTarget.WorldPosition.WithZ(30f);
        Vector3 direction = (upAttackPosition - AttackPosition.WorldPosition).Normal;
        var directionRotate = Rotation.LookAt(new Vector3(direction.x, direction.y, direction.z)) * Vector3.Forward;

        _tr = Scene.Trace.Ray(new Ray(origin, directionRotate), LostRadius)
            .IgnoreGameObject(GameObject)
            .Run();

        var shootDir = (_tr.Hit ? (_tr.EndPosition - AttackPosition.WorldPosition) : Vector3.Forward).Normal;
        var spawnPos = AttackPosition.WorldPosition;
        var spawnRot = Rotation.LookAt(_attackTarget.WorldPosition);

        var obj = ProjectilePrefab.Clone(spawnPos, spawnRot);
        var projectile = obj.GetComponent<Bullet>();
        projectile.Direction = _tr.Direction;
        projectile.Owner = GameObject;
        projectile.Weapon = AttackPosition.Parent;

        Sound.Play(AttackSound, AttackPosition.WorldPosition);

        if (_tr.Hit)
        {
            var damagable = _tr.GameObject.GetComponentInChildren<IDamageable>();

            if (damagable is not null)
            {
                damagable.OnDamage(new(AttackDamage, projectile.Owner, projectile.Weapon));
            }
        }
    }

    private void SearchTarget()
    {
        if (CurrentState != VampireState.Idle) return;

        _searchSphere = new Sphere(WorldPosition, SearchRadius);

        var objectInSphere = Scene.FindInPhysics(_searchSphere);

        foreach (var item in objectInSphere)
        {
            if (!IsFriend(item))
                if (IsTargetVisible(item, SearchRadius))
                    SetTarget(item);
        }
    }

    private void RandomRotate()
    {
        if (CurrentState != VampireState.Idle) return;

        var random = new Random();
        var angle = random.NextDouble() * Math.PI * 2;
        var rotation = Rotation.FromYaw((float)(angle * (360 / Math.PI)));

        WorldRotation = rotation;
    }

    public override void OnDamage(in DamageInfo dmgInfo)
    {
        Health -= dmgInfo.Damage;
        _lastAttacker = dmgInfo.Attacker;

        Renderer.Tint = _red;
        ResetBlockDamageTimer();

        Sound.Play(TakeDamageSound, WorldPosition);

        SetTarget(dmgInfo.Attacker);

        AllowMoving();

        if (Health <= 0)
            Die();
    }

    public override void Die()
    {
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

    private Vector3 GetRandomPoint()
    {
        var random = new Random();
        var angle = random.NextDouble() * Math.PI * 2;
        var x = _spawnPosition.x + (float)(Math.Cos(angle) * MovingRadius);
        var y = _spawnPosition.y + (float)(Math.Sin(angle) * MovingRadius);

        var randomPoint = new Vector3(x, y, _spawnPosition.z);

        return randomPoint;
    }

    private bool IsMovingAllowed(Vector3 point)
    {
        var rotate = Rotation.FromToRotation(point, Vector3.Up);
        var direction = rotate * Vector3.Forward;

        var tr = Scene.Trace.Ray(new Ray(point, direction), 1)
           .Radius(10)
           .Run();

        return !tr.Hit;
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

    public void SetTarget(GameObject target)
    {
        _attackTarget = target;
        CurrentState = VampireState.Attack;
    }

    private void AllowMoving() => _delayMovingTimer = 0;
    private void ResetMovingTimer() => _delayMovingTimer = MovingDelay;
    private void ResetAttackTimer() => _delayAttackTimer = AttackDelay;
    private void ResetBlockDamageTimer() => _delayBlockDamageTimer = _delayBlockDamage;
}