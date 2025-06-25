public sealed class CutscenePoint : Component
{
    [Property] public float Delay { get; set; } = 5f;

    protected override void DrawGizmos()
    {
        Gizmo.Transform = global::Transform.Zero;

        Gizmo.Draw.Color = Color.Green;
        Gizmo.Draw.LineSphere(WorldPosition, 2f);
    }
}