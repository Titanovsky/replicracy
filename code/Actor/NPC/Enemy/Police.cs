using Sandbox;

public class Police : EnemyBase
{
    [Property] protected EmotionsController EmotionsController { get; set; }
    [Property] protected SkinnedModelRenderer Renderer { get; set; }
    [Property] protected NavMeshAgent NavMeshAgent { get; set; }
    [Property] protected float SearchRadius { get; set; } = 500f;
    [Property] protected float LostRadius { get; set; } = 700f;
    [Property] protected float MovingRadius { get; set; } = 150f;
    [Property] protected float MoveDelay { get; set; } = 10f;

    [Property][Category("Weapon")] protected float BulletDamage { get; set; } = 3f;
    [Property][Category("Weapon")] protected float MagazineAmount { get; set; } = 1f;
    [Property][Category("Weapon")] protected float ShootDelay { get; set; } = 0.25f;
    [Property][Category("Weapon")] protected float ReloadDelay { get; set; } = 5f;
    [Property][Category("Weapon")] protected SoundEvent ShootSound { get; set; }

    [Property] protected GameObject AttackPosition { get; set; }
    [Property] protected GameObject ProjectilePrefab { get; set; }

    private PoliceState CurrentState { get; set; }

    private Vector3 _spawnPosition;
    private Vector3 _randomPointMoving;

    private RealTimeUntil _delayMovingTimer;
    private RealTimeUntil _delayShootTimer;
    private RealTimeUntil _delayReloadTimer;

    private GameObject _attackTarget;
    private GameObject _lastAttacker;

    private Sphere searchSphere;

    private int shootCount = 0;

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
        if (CurrentState != PoliceState.Idle) return;

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
        if (CurrentState != PoliceState.Idle) return;

        Vector3 direction = (_randomPointMoving - WorldPosition).Normal;

        RotateTo(direction);
    }

    private void Attack()
    {
        if (CurrentState != PoliceState.Attack) return;

        if (!_delayReloadTimer) return;

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

    SceneTraceResult tr;

    private void Shoot()
    {
        if (!_delayShootTimer) return;

        shootCount++;

        var origin = AttackPosition.WorldPosition;
        var endPos = _attackTarget.WorldPosition + new Vector3(0, 0, 40);

        tr = Scene.Trace.Ray(origin, endPos)
            .IgnoreGameObject(GameObject)
            .Run();

        var shootDir = (tr.Hit ? (tr.EndPosition - AttackPosition.WorldPosition) : Vector3.Forward).Normal;
        var spawnPos = AttackPosition.WorldPosition;
        var spawnRot = Rotation.LookAt(_attackTarget.WorldPosition);

        var obj = ProjectilePrefab.Clone(spawnPos, spawnRot);
        var projectile = obj.GetComponent<Bullet>();
        projectile.Damage = BulletDamage;
        projectile.Direction = tr.Direction;
        projectile.Owner = Player.Instance.GameObject;
        projectile.Weapon = AttackPosition.Parent;

        Sound.Play(ShootSound, AttackPosition.WorldPosition);

        if (tr.Hit)
        {
            var damagable = tr.Collider.GameObject.GetComponent<IDamageable>();

            if (damagable is not null)
            {
                damagable.OnDamage(new(projectile.Damage, projectile.Owner, projectile.Weapon));
            }
        }

        if (shootCount >= MagazineAmount)
        {
            shootCount = 0;

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
}
