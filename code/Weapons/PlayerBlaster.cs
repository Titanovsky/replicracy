public class PlayerBlaster : Component
{
    [Property] public GameObject ProjectileSpawner { get; set; }
    [Property] public GameObject ProjectilePrefab { get; set; }
    [Property] public SoundEvent ShotSound { get; set; }
    [Property] public float ShotDelay { get; set; } = .64f;
    private TimeUntil _shotDelayTimer;

    public void Shot()
    {
        if (!_shotDelayTimer) return;

        var screenCenter = new Vector2(Screen.Width, Screen.Height) * 0.5f;
        var ray = Scene.Camera.ScreenPixelToRay(screenCenter);

        var distance = 4096f; //todo move to constant property
        var tr = Scene.Trace.Ray(ray.Position, ray.Position + ray.Forward * distance).Run();

        if (tr.Hit)
            Log.Info(tr.Collider.GameObject);

        var shootDir = (tr.Hit ? (tr.EndPosition - ProjectileSpawner.WorldPosition) : ray.Forward).Normal;
        var spawnPos = ProjectileSpawner.WorldPosition;
        var spawnRot = Rotation.LookAt(shootDir);

        var obj = ProjectilePrefab.Clone(spawnPos, spawnRot);
        var projectile = obj.GetComponent<Bullet>();
        projectile.Owner = GameObject;
        projectile.Weapon = ProjectileSpawner.Parent;

        Sound.Play(ShotSound, ProjectileSpawner.WorldPosition);

        _shotDelayTimer = ShotDelay;

        AnimShot();
    }

    private void AnimShot()
    {
        Player.Instance.PlayerController.Renderer.Set("b_attack", true);
    }

    private void InputShot()
    {
        if (Input.Down("Attack1"))
            Shot();
    }

    private void Prepare()
    {
        //todo
    }

    protected override void OnStart()
    {
        Prepare();
    }

    protected override void OnUpdate()
    {
        InputShot();
    }
}