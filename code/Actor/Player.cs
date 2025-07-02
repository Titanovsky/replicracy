using Sandbox;
using Sandbox.Services;
using Sandbox.Utility;
using System;
using System.Linq;

public sealed class Player : Component, Component.IDamageable, PlayerController.IEvents
{
    public static Player Instance { get; private set; }

    [Property] public float PlayerUseRay { get; set; } = 130f;

    [Property] public float MaxHealth { get; set; } = 100f;
    [Property] public float Health { get; set; } = 0f;
    [Property] public int Dna { get; set; } = 0;
    [Property] public int Frags { get; set; } = 0;
    [Property] public bool GodMode { get; set; } = false;
    public int CollectDna { get; set; } = 0;
    public int CollectSecrets { get; set; } = 0;

    [Property] public PlayerController PlayerController { get; set; }
    [Property] public ReplicantController ReplicantController { get; set; }
    [Property] public Hint Hint { get; set; }
    [Property] public HeaderLevel HeaderLevel { get; set; }
    [Property] public UsableUI UsablePanel { get; set; }
    [Property] public Fade Fade { get; set; }
    [Property] public ErrorMessage ErroreMessage { get; set; }
    [Property] public Wipmessage WIPMessage { get; set; }
    [Property] public SecretsHud SecretsHud { get; set; }

    private SceneTraceResult _traceResult { get; set; }
    private IUsable _playerViewedObject {  get; set; }

    public Action<SceneTraceResult> OnSpecified { get; set; }

    private Vector3 _spawnPos;

    //todo: fix after, it's not important fuckup
    //public void PostCameraSetup(CameraComponent cam)
    //{
    //    Log.Info($"{Scene.Camera.WorldPosition}");
    //}

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
        _spawnPos = WorldPosition;

        Health = MaxHealth;

        if (!PlayerController.IsValid())
            PlayerController = GetComponent<PlayerController>();

        if (!Hint.IsValid())
            Hint = GetComponent<Hint>();

        if (!HeaderLevel.IsValid())
            HeaderLevel = GetComponent<HeaderLevel>();

        if (!UsablePanel.IsValid())
            UsablePanel = GetComponent<UsableUI>();

        if (!Fade.IsValid())
            Fade = GetComponent<Fade>();

        if (!ErroreMessage.IsValid())
            ErroreMessage = GetComponent<ErrorMessage>();

        if (!WIPMessage.IsValid())
            WIPMessage = GetComponent<Wipmessage>();
    }

    public void Specify()
    {
        if (ReplicantController.Replicants.Count == 0) return;

        var screenCenter = new Vector2(Screen.Width, Screen.Height) * .5f;
        var ray = Scene.Camera.ScreenPixelToRay(screenCenter);

        _traceResult = Scene.Trace.Ray(ray.Position, ray.Position + ray.Forward * 10000f)
            .Size(BBox.FromPositionAndSize(-8, 8))
            .IgnoreGameObject(GameObject)
            .Run();

        if (!_traceResult.Hit) return;

        OnSpecified?.Invoke(_traceResult);

        if (CanAchievement() && _traceResult.Collider.GameObject.GetComponent<Loot>().IsValid())
            Achievements.Unlock("spermabank");
    }

    public void Use()
    {
        if (_playerViewedObject == null) return;

        _playerViewedObject.Use();
    }

    [Property] public SoundEvent ErrorSound;
    public void Error(string str = "")
    {
        if (str != "")
            ErroreMessage.Show(str, 6f);

        Sound.Play(ErrorSound);
    }

    public void Die()
    {
        Log.Info($"Restart");

        Scene.Load(LevelManager.Instance.CurrentLevel.CurrentLevelScene);
    }

    public bool CanAchievement()
    {
        return !Game.IsEditor;
    }

    private void PlayerView()
    {
        var screenCenter = new Vector2(Screen.Width, Screen.Height) * .5f;
        var ray = Scene.Camera.ScreenPixelToRay(screenCenter);

        _traceResult = Scene.Trace.Ray(ray.Position, ray.Position + ray.Forward * PlayerUseRay)
            .IgnoreGameObject(GameObject)
            .Run();

        if (GlobalSettings.IsDebug && _traceResult.Hit)
        {
            Gizmo.Draw.Color = Color.Yellow;
            Gizmo.Draw.LineThickness = 4;
            Gizmo.Draw.Line(_traceResult.StartPosition - new Vector3(0, 0, 1f), _traceResult.HitPosition);
        }

        IUsable newTarget = _traceResult.Hit
                           ? _traceResult.GameObject.Components.Get<IUsable>()
                           : null;

        if (newTarget == _playerViewedObject)
            return;

        _playerViewedObject?.DisableHightlight();

        _playerViewedObject = newTarget;

        _playerViewedObject?.EnableHightlight();
        UsablePanel.UsableMessage = _playerViewedObject?.GetUsableText();
    }

    private void CheckInput()
    {
        if (Input.Pressed("Attack2"))
            Specify();

        if (Input.Pressed("Use"))
            Use();
    }

    private void CheckFlyDown()
    {
        if (WorldPosition.z <= LevelManager.Instance.CurrentLevel.MinDangerousZ)
            WorldPosition = _spawnPos;
    }

    private void DrawSpecified()
    {
        //Gizmo.Draw.Color = Color.White.WithAlpha(0.1f);
        //Gizmo.Draw.LineThickness = 4;
        //Gizmo.Draw.Line(_traceResult.StartPosition, _traceResult.EndPosition);

        //Gizmo.Draw.Color = Color.Green;
        //Gizmo.Draw.Line(_traceResult.EndPosition, _traceResult.EndPosition + _traceResult.Normal * 1.0f);
    }

    [Property] public SoundEvent HurtsSound;
    private void TakeDamage(in DamageInfo dmgInfo)
    {
        var damage = dmgInfo.Damage;

        Health -= damage;
        Sound.Play(HurtsSound);

        Log.Info($"{GameObject} take damage {damage}f by {dmgInfo.Attacker}");

        if (Health <= 0)
            Die();
    }

    protected override void OnAwake()
    {
        CreateSingleton();
    }

    protected override void OnDestroy()
    {
        DestroySingleton();
    }

    protected override void OnStart()
    {
        Prepare();
    }

    protected override void OnUpdate()
    {
        PlayerView();
        CheckInput();
        DrawSpecified();
        CheckFlyDown();
    }

    public void OnDamage(in DamageInfo dmgInfo)
    {
        TakeDamage(in dmgInfo);
    }
}
