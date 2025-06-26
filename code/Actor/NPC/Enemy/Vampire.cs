using Sandbox;
using System;

public sealed class Vampire : EnemyBase
{
    [Property] public SkinnedModelRenderer Renderer { get; set; }
    [Property] public float SearchRadius { get; set; } = 500f;
    [Property] public float LostRadius { get; set; } = 700f;
    [Property] public float MovingDelay { get; set; } = 5f;
    [Property] public float MovingRadius { get; set; } = 150f;
    [Property] public float AttackDamage { get; set; } = 10f;
    [Property] public float AttackDelay { get; set; } = 6f;
    [Property] public SoundEvent AttackSound { get; set; }
    [Property] public GameObject AttackPosition { get; set; }
    [Property] public GameObject ProjectilePrefab { get; set; }

    private VampireState CurrentState { get; set; }

    private Vector3 _spawnPosition;
    private RealTimeUntil _delayMovingTimer;
    private RealTimeUntil _delayAttackTimer;

    private GameObject _attackTarget;
    private GameObject _lastAttacker;

    Sphere searchSphere;

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

        var tr = Scene.Trace.Ray(AttackPosition.WorldPosition, _attackTarget.WorldPosition).Run();

        var shootDir = (tr.Hit ? (tr.EndPosition - AttackPosition.WorldPosition) : Vector3.Forward).Normal;
        var spawnPos = AttackPosition.WorldPosition;
        var spawnRot = Rotation.LookAt(_attackTarget.WorldPosition);

        var obj = ProjectilePrefab.Clone(spawnPos, spawnRot);
        var projectile = obj.GetComponent<Bullet>();
        projectile.Direction = tr.Direction;
        projectile.Owner = Player.Instance.GameObject;
        projectile.Weapon = AttackPosition.Parent;

        Sound.Play(AttackSound, AttackPosition.WorldPosition);

        if (tr.Hit)
        {
            Log.Info($"[Vampire] hit {tr.Collider.GameObject}");

            var damagable = tr.Collider.GameObject.GetComponent<IDamageable>();
            if (damagable is not null)
            {
                damagable.OnDamage(new(projectile.Damage, projectile.Owner, projectile.Weapon));
            }
        }
    }

    private void SearchTarget()
    {
        if (CurrentState != VampireState.Idle) return;

        searchSphere = new Sphere(WorldPosition, SearchRadius);

        var objectInSphere = Scene.FindInPhysics(searchSphere);

        foreach (var item in objectInSphere)
        {
            if (!IsFriend(item))
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
            ply.HeaderLevel.Show();
        }

        DestroyGameObject();
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
}