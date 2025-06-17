using Sandbox;
using Sandbox.Utility;
using System;

public sealed class Player : Component
{
    public static Player Instance { get; private set; }

    [Property] public float MaxHealth { get; set; } = 0f;
    [Property] public float Health { get; set; } = 0f;
    [Property] public int Dna { get; set; } = 0;
    [Property] public PlayerController PlayerController { get; set; }
    [Property] public GameObject Hui { get; set; }

    private SceneTraceResult _traceResult;

    public Action<Vector3> OnSpecified { get; set; }

    private void Prepare()
    {
        MaxHealth = 100f;
        Health = MaxHealth;
        Dna = 0;

        if (!PlayerController.IsValid())
            PlayerController = GetComponent<PlayerController>();
    }

    private void CreateSingleton()
    {
        if (Instance is null)
            Instance = this;
    }

    public void Specify()
    {
        _traceResult = Scene.Trace.Ray(ray: PlayerController.EyeTransform.ForwardRay, 10000f)
            .Size(BBox.FromPositionAndSize(-8, 8))
            .IgnoreGameObject(GameObject)
            .Run();

        if (!_traceResult.Hit) return;

        OnSpecified?.Invoke(_traceResult.HitPosition);

        //todo Remove
        DrawAvatar();
    }

    private void CheckSpecify()
    {
        if (Input.Pressed("Use"))
            Specify();
    }

    private void DrawSpecified()
    {
        Gizmo.Draw.Color = Color.White.WithAlpha(0.1f);
        Gizmo.Draw.LineThickness = 4;
        Gizmo.Draw.Line(_traceResult.StartPosition, _traceResult.EndPosition);

        Gizmo.Draw.Color = Color.Green;
        Gizmo.Draw.Line(_traceResult.EndPosition, _traceResult.EndPosition + _traceResult.Normal * 1.0f);
    }

    private void CustomAnim()
    {
        var renderer = PlayerController.Renderer;
        renderer.Set("b_swim", true); // here
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
        CheckSpecify();
        DrawSpecified();
    }

    protected override void OnDestroy()
    {
        Instance = null;
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
