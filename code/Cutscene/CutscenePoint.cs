public sealed class CutscenePoint : Component
{
    [Property] public float Delay { get; set; } = 5f;
    [Property] public float SpeedToPoint { get; set; } = 3f;

    protected override void DrawGizmos()
    {
        Gizmo.Transform = global::Transform.Zero;

        Gizmo.Draw.Color = Color.Green;
        Gizmo.Draw.LineSphere(WorldPosition, 2f);

        Gizmo.Draw.Color = Color.White;
        Gizmo.Draw.Arrow(WorldPosition, WorldPosition + WorldRotation.Forward * 13f, 6f, 2.5f);
    }
}