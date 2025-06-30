using Sandbox;
using System;
using static Zombie;

public sealed class Demon : EnemyBase
{
    [Property] public SkinnedModelRenderer Renderer { get; set; }
    [Property] public NavMeshAgent NavMeshAgent { get; set; }
    [Property] public float SearchRadius { get; set; } = 500f;
    [Property] public float LostRadius { get; set; } = 700f;
    [Property] public float MovingDelay { get; set; } = 5f;
    [Property] public float MovingStartPosRadius { get; set; } = 150f;
    [Property] public int DNA { get; set; } = 1;
    [Property] public SoundEvent TakeDamageSound;

    [Property][Category("Weapon")] public float AttackDamage { get; set; } = 10f;
    [Property][Category("Weapon")] public float AttackDelay { get; set; } = 6f;
    [Property][Category("Weapon")] public float AttackDistance { get; set; } = 200f;
    [Property][Category("Weapon")] public SoundEvent AttackSound { get; set; }
    [Property][Category("Weapon")] public GameObject AttackPosition { get; set; }
    [Property][Category("Weapon")] public GameObject ProjectilePrefab { get; set; }

    private DemonState CurrentState { get; set; }

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

    public enum DemonState
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
        if (CurrentState != DemonState.Idle) return;

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
        if (CurrentState != DemonState.Idle) return;

        _searchSphere = new Sphere(WorldPosition, SearchRadius);

        var objectInSphere = Scene.FindInPhysics(_searchSphere);

        foreach (var item in objectInSphere)
        {
            if (!IsFriend(item))
                SetTarget(item);
        }
    }

    private void RotateToMovingPoint()
    {
        if (CurrentState != DemonState.Idle) return;

        Vector3 direction = (_randomPointMoving - WorldPosition).Normal;

        RotateTo(direction);
    }

    private void Attack()
    {
        if (CurrentState != DemonState.Attack) return;

        if (!_delayAttackTimer) return;

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
        projectile.Owner = Player.Instance.GameObject;
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

        ResetAttackTimer();
    }

    private void FollowToTarger()
    {
        if (CurrentState != DemonState.Attack) return;

        var distanceForAttack = AttackDistance;
        var postion = _attackTarget.WorldPosition - new Vector3(distanceForAttack, distanceForAttack, 0);

        NavMeshAgent.MoveTo(postion);
    }

    private void CheckDistanceToTarget()
    {
        if (CurrentState != DemonState.Attack) return;

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
        Log.Info($"[Demon] Die from {_lastAttacker}");

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
        CurrentState = DemonState.Idle;
    }

    public void SetAttackState()
    {
        CurrentState = DemonState.Attack;
    }

    public void SetTarget(GameObject target)
    {
        _attackTarget = target;

        SetAttackState();
    }

    private void ResetMovingTimer() => _delayMovingTimer = MovingDelay;
    private void ResetAttackTimer() => _delayAttackTimer = AttackDelay;
    private void ResetBlockDamageTimer() => _delayBlockDamageTimer = _delayBlockDamage;
}
