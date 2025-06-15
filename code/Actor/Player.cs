using Sandbox;
using System;

public sealed class Player : Component
{
    public static Player Instance { get; private set; }

    [Property] public float MaxHealth { get; set; } = 0f;
    [Property] public float Health { get; set; } = 0f;
    [Property] public int Dna { get; set; } = 0;
    [Property] public CameraComponent Camera { get; set; }

    public Action<Vector3> OnSpecified { get; set; }

    private void Prepare()
    {
        MaxHealth = 100f;
        Health = MaxHealth;
        Dna = 0;

        Camera = Scene.Camera;
    }

    private void CreateSingleton()
    {
        if (Instance is null)
            Instance = this;
    }

    public void Specify()
    {
        var tr = Scene.Trace.Ray(Camera.WorldPosition, Camera.WorldTransform.Forward * 10000f)
            .UseHitboxes(true)
            .Size(BBox.FromPositionAndSize(-5, 5))
            .Run();

        if (!tr.Hit) return;

        OnSpecified?.Invoke(tr.HitPosition);

        WorldPosition = tr.HitPosition;
    }

    private void CheckSpecify()
    {
        if (Input.Pressed("Use"))
            Specify();
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
    }
}
