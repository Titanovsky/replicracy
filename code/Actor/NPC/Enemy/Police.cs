using Sandbox;
using static System.Net.Mime.MediaTypeNames;

public class Police : EnemyBase
{
    [Property] protected EmotionsController EmotionsController { get; set; }
    [Property] protected SkinnedModelRenderer Renderer { get; set; }
    [Property] protected NavMeshAgent NavMeshAgent { get; set; }
    [Property] protected float SearchRadius { get; set; } = 500f;
    [Property] protected float LostRadius { get; set; } = 700f;
    [Property] protected float MovingRadius { get; set; } = 150f;
    [Property] protected float MoveDelay { get; set; } = 10f;
    [Property] protected int DNA { get; set; } = 1;
    [Property] public SoundEvent TakeDamageSound { get; set; }

    [Property][Category("Weapon")] protected float BulletDamage { get; set; } = 3f;
    [Property][Category("Weapon")] protected int MagazineAmount { get; set; } = 1;
    [Property][Category("Weapon")] protected float ShootDelay { get; set; } = 0.25f;
    [Property][Category("Weapon")] protected float ReloadDelay { get; set; } = 5f;
    [Property][Category("Weapon")] protected SoundEvent ShootSound { get; set; }

    [Property] protected GameObject AttackPosition { get; set; }
    [Property] protected GameObject ProjectilePrefab { get; set; }

    private PoliceState CurrentState { get; set; }

    private Vector3 _spawnPosition;
    private Vector3 _randomPointMoving;

    private RealTimeUntil _delayMovingTimer = 0;
    private RealTimeUntil _delayShootTimer = 0;
    private RealTimeUntil _delayReloadTimer = 0;

    private GameObject _attackTarget;
    private GameObject _lastAttacker;

    private Sphere _searchSphere;
    private SceneTraceResult _tr;

    private int _shootCount = 0;

    private float _delayBlockDamage = 0.3f;
    private Color32 _white = Color.White;
    private Color32 _red = Color.Red;
    private TimeUntil _delayBlockDamageTimer;

    public enum PoliceState
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

        ResetColor();
    }

    private void Moving()
    {
        if (!_delayMovingTimer) return;

        if (Scene.NavMesh.GetRandomPoint(_spawnPosition, MovingRadius).HasValue)
        {
            _randomPointMoving = Scene.NavMesh.GetRandomPoint(_spawnPosition, MovingRadius).Value;
            NavMeshAgent.MoveTo(_randomPointMoving);
        }

        ResetMovingTimer();
    }

    private void SearchTarget()
    {
        if (!GameObject.IsValid()) return;
        if (CurrentState != PoliceState.Idle) return;

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
        if (CurrentState != PoliceState.Idle) return;

        Vector3 direction = (_randomPointMoving - WorldPosition).Normal;

        RotateTo(direction);
    }

    private void Attack()
    {
        if (CurrentState != PoliceState.Attack) return;

        if (!_delayReloadTimer) return;

        if (!IsTargetVisible(_attackTarget, LostRadius)) return;

        Shoot();
    }

    private void CheckDistanceToTarget()
    {
        if (CurrentState != PoliceState.Attack) return;

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
        if (CurrentState != PoliceState.Attack) return;

        Vector3 direction = (_attackTarget.WorldPosition - WorldPosition).Normal;

        RotateTo(direction);
    }

    private void Shoot()
    {
        if (!_delayShootTimer) return;

        _shootCount++;

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

        Sound.Play(ShootSound, AttackPosition.WorldPosition);

        if (_tr.Hit)
        {
            var damagable = _tr.GameObject.GetComponentInChildren<IDamageable>();

            if (damagable is not null)
            {
                damagable.OnDamage(new(BulletDamage, projectile.Owner, projectile.Weapon));
            }
        }

        if (_shootCount >= MagazineAmount)
        {
            _shootCount = 0;

            ResetReloadTimer();
        }

        ResetShootTimer();
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
        Log.Info($"[Police] Die from {_lastAttacker}");

        if (_lastAttacker == Player.Instance.GameObject || _lastAttacker.Tags.Has("replicant"))
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

        if (target.Tags.Has("undead"))
            return false;

        if (target.Tags.Has("allien"))
            return false;

        return base.IsFriend(target);
    }

    public void SetIdleState()
    {
        _attackTarget = null;

        CurrentState = PoliceState.Idle;
        EmotionsController.SetEmotion(EmotionsController.Emotions.Idle);

        MoveDelay = MoveDelay * 2f;
    }

    public void SetAttackState()
    {
        CurrentState = PoliceState.Attack;
        EmotionsController.SetEmotion(EmotionsController.Emotions.Angry);

        MoveDelay = MoveDelay / 2f;
    }

    public void SetTarget(GameObject target)
    {
        _attackTarget = target;

        SetAttackState();
    }

    private void ResetMovingTimer() => _delayMovingTimer = MoveDelay;
    private void ResetShootTimer() => _delayShootTimer = ShootDelay;
    private void ResetReloadTimer() => _delayReloadTimer = ReloadDelay;
    private void ResetBlockDamageTimer() => _delayBlockDamageTimer = _delayBlockDamage;
}