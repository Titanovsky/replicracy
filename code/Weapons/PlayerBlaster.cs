public class PlayerBlaster : Component
{
    [Property] public GameObject ProjectileSpawner { get; set; }
    [Property] public GameObject ProjectilePrefab { get; set; }
    [Property] public SoundEvent ShotSound { get; set; }
    [Property] public float ShotDelay { get; set; } = .64f;
    [Property] public float ShotDistance { get; set; } = 2048f;
    private TimeUntil _shotDelayTimer;

    private SceneTraceResult _tr;

    public void Shot()
    {
        if (!_shotDelayTimer) return;
        _shotDelayTimer = ShotDelay;

        var screenCenter = new Vector2(Screen.Width, Screen.Height) * .5f;
        var ray = Scene.Camera.ScreenPixelToRay(screenCenter);
        var tr = Scene.Trace.Ray(ray.Position, ray.Position + ray.Forward * ShotDistance).Run();

        var shootDir = (tr.Hit ? (tr.EndPosition - ProjectileSpawner.WorldPosition) : ray.Forward).Normal;
        var spawnPos = ProjectileSpawner.WorldPosition;
        var spawnRot = Rotation.LookAt(shootDir);

        _tr = tr;

        var obj = ProjectilePrefab.Clone(spawnPos, spawnRot);
        var projectile = obj.GetComponent<Bullet>();
        projectile.Direction = tr.Direction;
        projectile.Owner = Player.Instance.GameObject;
        projectile.Weapon = ProjectileSpawner.Parent;

        Sound.Play(ShotSound, ProjectileSpawner.WorldPosition);

        AnimShot();

        if (tr.Hit)
        {
            Log.Info($"[Blaster] hit {tr.Collider.GameObject}");

            var damagable = tr.Collider.GameObject.GetComponent<IDamageable>();
            if (damagable is not null)
            {
                damagable.OnDamage(new(projectile.Damage, projectile.Owner, projectile.Weapon));
            }
        }
    }

    private void Show()
    {
        if (!GlobalSettings.IsDebug || !_tr.Hit) return;

        Gizmo.Draw.Color = Color.Red;
        Gizmo.Draw.LineThickness = 1f;
        Gizmo.Draw.Arrow(_tr.StartPosition, _tr.EndPosition, 12, 5);
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
        Show();
    }
}