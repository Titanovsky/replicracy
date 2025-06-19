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

    private SceneTraceResult _traceResult;
    private IUsable _playerViewedObject;
    private PrefabFile _panelComponent;

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
    }

    public void Specify()
    {
        _traceResult = Scene.Trace.Ray(ray: PlayerController.EyeTransform.ForwardRay, 10000f)
            .Size(BBox.FromPositionAndSize(-8, 8))
            .IgnoreGameObject(GameObject)
            .Run();

        if (!_traceResult.Hit) return;

        OnSpecified?.Invoke(_traceResult);

        //todo Remove
        DrawAvatar();
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

        if (!_traceResult.Hit)
        {
            if (_playerViewedObject != null)
            {
                _playerViewedObject.DisableHightlight();
                _playerViewedObject = null;
            }

            return;
        }

        IUsable usable = _traceResult.GameObject.Components.Get<IUsable>();
        if (usable == null) return;

        _playerViewedObject = usable;
        _playerViewedObject.EnableHightlight();
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

        var pos = BlasterProjectileSpawner.WorldPosition;
        var rot = PlayerController.EyeAngles.ToRotation();
        var obj = BlasterProjectilePrefab.Clone(pos, rot);
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
        Gizmo.Draw.Color = Color.White.WithAlpha(0.1f);
        Gizmo.Draw.LineThickness = 4;
        Gizmo.Draw.Line(_traceResult.StartPosition, _traceResult.EndPosition);

        Gizmo.Draw.Color = Color.Green;
        Gizmo.Draw.Line(_traceResult.EndPosition, _traceResult.EndPosition + _traceResult.Normal * 1.0f);
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

    private void DrawAvatar()
    {
        GameObject avatarObject = new GameObject(true, "Avatar");

        avatarObject.WorldPosition = _traceResult.HitPosition;
        avatarObject.WorldRotation = Rotation.FromToRotation(Vector3.Forward, -_traceResult.Normal);

        DecalAvatar decalAvatar = avatarObject.AddComponent<DecalAvatar>();
        decalAvatar.DrawAvatarDecal(Steam.SteamId.ToString());
    }
}
