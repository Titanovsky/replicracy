using Sandbox;
using Sandbox.Utility;
using System;

public sealed class Player : Component
{
    public static Player Instance { get; private set; }

    [Property] public float PlayerUseRay { get; set; } = 100f;
    [Property] public float MaxHealth { get; set; } = 100f;
    [Property] public float Health { get; set; } = 0f;
    [Property] public int Dna { get; set; } = 0;
    [Property] public PlayerController PlayerController { get; set; }
    [Property] public Hint Hint { get; set; }
    [Property] public HeaderLevel HeaderLevel { get; set; }
    [Property] private UsableUI _usablePanel { get; set; }

    private SceneTraceResult _traceResult { get; set; }
    private IUsable _playerViewedObject {  get; set; }

    public Action<SceneTraceResult> OnSpecified { get; set; }

    //todo: move to class PlayerBlaster
    [Property] public GameObject BlasterProjectileSpawner { get; set; }
    [Property] public GameObject BlasterProjectilePrefab { get; set; }
    [Property] public SoundEvent BlasterShotSound { get; set; }
    [Property] public float BlasterShotDelay { get; set; } = .64f;
    private TimeUntil _blasterShotDelay;

    private void CreateSingleton()
    {
        if (Instance is null)
            Instance = this;
    }

    private void DestroySingleton()
    {
        Instance = null;
    }

    private void Prepare()
    {
        Health = MaxHealth;
        Dna = 0;

        if (!PlayerController.IsValid())
            PlayerController = GetComponent<PlayerController>();

        if (!Hint.IsValid())
            Hint = GetComponent<Hint>();

        if (!HeaderLevel.IsValid())
            HeaderLevel = GetComponent<HeaderLevel>();
    }

    public void Specify()
    {
        _traceResult = Scene.Trace.Ray(ray: PlayerController.EyeTransform.ForwardRay, 10000f)
            .Size(BBox.FromPositionAndSize(-8, 8))
            .IgnoreGameObject(GameObject)
            .Run();

        if (!_traceResult.Hit) return;

        OnSpecified?.Invoke(_traceResult);
    }

    public void Use()
    {
        if (_playerViewedObject == null) return;

        _playerViewedObject.Use();
    }

    private void PlayerView()
    {
        _traceResult = Scene.Trace.Ray(ray: PlayerController.EyeTransform.ForwardRay, PlayerUseRay)
        .Size(BBox.FromPositionAndSize(-8, 8))
        .IgnoreGameObject(GameObject)
        .Run();

        IUsable newTarget = _traceResult.Hit
                           ? _traceResult.GameObject.Components.Get<IUsable>()
                           : null;

        if (newTarget == _playerViewedObject)
            return;

        _playerViewedObject?.DisableHightlight();

        _playerViewedObject = newTarget;

        _playerViewedObject?.EnableHightlight();
        _usablePanel.UsableMessage = _playerViewedObject?.GetUsableText();
    }

    private void CheckInput()
    {
        if (Input.Pressed("Specify"))
            Specify();

        if (Input.Pressed("Use"))
            Use();
    }

    private void Shot()
    {
        if (!_blasterShotDelay) return;

        var screenCenter = new Vector2(Screen.Width, Screen.Height) * 0.5f;
        var ray = Scene.Camera.ScreenPixelToRay(screenCenter);

        var distance = 4096f; //todo move to constant property
        var tr = Scene.Trace.Ray(ray.Position, ray.Position + ray.Forward * distance).Run();

        if (tr.Hit)
            Log.Info(tr.Collider.GameObject);

        var shootDir = (tr.Hit ? (tr.EndPosition - BlasterProjectileSpawner.WorldPosition) : ray.Forward).Normal;
        var spawnPos = BlasterProjectileSpawner.WorldPosition;
        var spawnRot = Rotation.LookAt(shootDir);

        var obj = BlasterProjectilePrefab.Clone(spawnPos, spawnRot);
        var projectile = obj.GetComponent<Bullet>();
        projectile.Owner = GameObject;
        projectile.Weapon = BlasterProjectileSpawner.Parent;

        Sound.Play(BlasterShotSound, BlasterProjectileSpawner.WorldPosition);

        _blasterShotDelay = BlasterShotDelay;

        PlayerController.Renderer.Set("b_attack", true);
    }

    private void InputShot()
    {
        if (Input.Down("Attack1"))
            Shot();
    }

    private void DrawSpecified()
    {
        //Gizmo.Draw.Color = Color.White.WithAlpha(0.1f);
        //Gizmo.Draw.LineThickness = 4;
        //Gizmo.Draw.Line(_traceResult.StartPosition, _traceResult.EndPosition);

        //Gizmo.Draw.Color = Color.Green;
        //Gizmo.Draw.Line(_traceResult.EndPosition, _traceResult.EndPosition + _traceResult.Normal * 1.0f);
    }

    protected override void OnStart()
    {
        Prepare();
    }

    protected override void OnAwake()
    {
        CreateSingleton();

    }

    protected override void OnUpdate()
    {
        PlayerView();
        CheckInput();
        DrawSpecified();
        InputShot();
    }

    protected override void OnDestroy()
    {
        DestroySingleton();
    }
}
