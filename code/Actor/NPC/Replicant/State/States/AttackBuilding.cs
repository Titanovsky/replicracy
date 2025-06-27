public class AttackBuilding : MovableState
{
    public AttackBuilding(Replicant replicant) : base(replicant) { }

    private SceneTraceResult tr;
    private GameObject _targetObject;

    public override void Enter()
    {
        if (Replicant.GetTargetPoint() == Vector3.Zero)
        {
            Replicant.replicantFSM.SetState<Idle>();
            return;
        }

        Replicant.MoveToPoint(Replicant.GetTargetPoint());
    }

    public override void Update()
    {
        UpdatedRotation(Replicant.GetTargetPoint());

        DrawSpecified();

        tr = Game.ActiveScene.Trace.Ray(new Ray(Replicant.GetEye().WorldPosition, Replicant.GetEye().WorldRotation.Forward), 30)
        .IgnoreGameObject(Replicant.GameObject)
        .Run();

        if (!tr.Hit) return;

        _targetObject = tr.GameObject;

        if (_targetObject.Tags.Has("building"))
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (!Replicant.IsAttackAllowed()) return;

        var building = _targetObject.Components.Get<Loot>();

        building.TakeDamage(Replicant.AttackDamage);

        Replicant.ReseAttackTimer();

        if (building.IsDead)
            Replicant.replicantFSM.SetState<ReturnToPlayer>();
    }

    private void DrawSpecified()
    {
        Gizmo.Draw.Color = Color.White.WithAlpha(0.1f);
        Gizmo.Draw.LineThickness = 4;
        Gizmo.Draw.Line(tr.StartPosition, tr.EndPosition);

        Gizmo.Draw.Color = Color.Green;
        Gizmo.Draw.Line(tr.EndPosition, tr.EndPosition + tr.Normal * 1.0f);
    }

}