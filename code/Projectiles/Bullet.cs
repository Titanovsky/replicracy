public sealed class Bullet : Component, Component.ITriggerListener
{
    [Property] public float Damage { get; set; } = 2f;
    [Property] public float TimeDie { get; set; } = 2f;
    [Property] public float MoveSpeed { get; set; } = 1.2f;
    [Property] public Vector3 Direction { get; set; } = Vector3.Zero;

    public GameObject Weapon { get; set; }
    public GameObject Owner { get; set; }

    private TimeUntil _TimeDieDelay { get; set; }

    private Vector3 _spawn;

    private void Prepare()
    {
        //Log.Info($"[Projectile] Spawn player bullet {GameObject}");

        _TimeDieDelay = TimeDie;
        _spawn = WorldPosition;

        //if (Direction == Vector3.Zero)
        //    Direction = WorldTransform.Forward;
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

    private void Show()
    {
        if (_spawn.IsNaN) return;

        Gizmo.Draw.Color = Color.Blue;
        Gizmo.Draw.LineThickness = 1f;
        Gizmo.Draw.Arrow(_spawn, WorldPosition, 6, 5);
    }

    protected override void OnStart()
    {
        Prepare();
    }

    protected override void OnUpdate()
	{
        DieDelay();
        Move();
        Show();
    }

    public void OnTriggerEnter(Collider other)
    {
        Log.Info($"[Bullet] trigger {other.GameObject}"); // for debug
        DestroyGameObject();
    }
}
