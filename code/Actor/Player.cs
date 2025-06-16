using Sandbox;
using System;

public sealed class Player : Component
{
    public static Player Instance { get; private set; }

    [Property] public float MaxHealth { get; set; } = 0f;
    [Property] public float Health { get; set; } = 0f;
    [Property] public int Dna { get; set; } = 0;
    [Property] public CameraComponent Camera { get; set; }
    [Property] public GameObject Body { get; set; }

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
        var tr = Scene.Trace.Ray(ray: Camera.WorldTransform.ForwardRay, 10000f)
            .Size(BBox.FromPositionAndSize(-6, 6))
            .Run();

        if (!tr.Hit) return;

        OnSpecified?.Invoke(tr.HitPosition);

        WorldPosition = tr.HitPosition;
        Log.Info($"{tr.Collider.GameObject}");
    }

    protected override void DrawGizmos()
    {
        Gizmo.Transform = global::Transform.Zero; // вот из-за этой хуйни гизмо не правильно позиционировался

        Gizmo.Draw.Color = Color.Green;
        Gizmo.Draw.Arrow(Body.WorldPosition, WorldPosition, 10f, 3f);
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
