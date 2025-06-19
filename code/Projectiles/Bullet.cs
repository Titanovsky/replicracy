public sealed class Bullet : Component, Component.ITriggerListener
{
    [Property] public float Damage { get; set; } = 2f;
    [Property] public float TimeDie { get; set; } = 2f;
    [Property] public float MoveSpeed { get; set; } = 1.2f;

    public GameObject Weapon { get; set; }
    public GameObject Owner { get; set; }

    private TimeUntil _TimeDieDelay { get; set; }

    private void Prepare()
    {
        Log.Info($"[Projectile] Spawn player bullet {GameObject}");

        _TimeDieDelay = TimeDie;
    }

    private void Move()
    {
        WorldPosition += WorldTransform.Forward * MoveSpeed;
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
        DestroyGameObject();

        var damagable = other.GetComponent<IDamageable>();
        if (damagable is not null)
        {
            damagable.OnDamage(new(Damage, Owner, Weapon));
        }
    }
}
