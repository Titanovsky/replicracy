public sealed class Bullet : Component, Component.ITriggerListener
{
    [Property] public float Damage { get; set; } = 2f;
    [Property] public float TimeDie { get; set; } = 2f;
    [Property] public float MoveSpeed { get; set; } = 1.2f;
    [Property] public Vector3 Direction { get; set; } = Vector3.Zero;

    public GameObject Weapon { get; set; }
    public GameObject Owner { get; set; }

    private TimeUntil _TimeDieDelay { get; set; }

    private void Prepare()
    {
        //Log.Info($"[Projectile] Spawn player bullet {GameObject}");

        _TimeDieDelay = TimeDie;

        if (Direction == Vector3.Zero)
            Direction = WorldTransform.Forward;
    }

    private void Move()
    {
        WorldPosition += Direction * MoveSpeed;
    }

    private void DieDelay()
    {
        if (!_TimeDieDelay) return;

        DestroyGameObject();
    }

    protected override void OnStart()
    {
        Prepare();
    }

    protected override void OnFixedUpdate()
	{
        DieDelay();
        Move();
    }

    public void OnTriggerEnter(Collider other)
    {
        Log.Info($"[Bullet] trigger {other.GameObject}"); // for debug
        DestroyGameObject();

        var damagable = other.GetComponent<IDamageable>();
        if (damagable is not null)
        {
            damagable.OnDamage(new(Damage, Owner, Weapon));
        }
    }
}
