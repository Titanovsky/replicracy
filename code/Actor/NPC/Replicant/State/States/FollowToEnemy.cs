public class FollowToEnemy : MovableState
{
    public FollowToEnemy(Replicant replicant) : base(replicant) { }

    private SceneTraceResult tr;

    public override void Update()
    {
        GoToEnemy();
        SearchEnemy();

        DrawSpecified();

        CheckTargetObjectIsDead();
    }

    private void GoToEnemy()
    {
        var target = Replicant.GetTargetObject();

        if (target == null) return;

        Replicant.MoveToPoint(target.WorldPosition);

        UpdatedRotation(target.WorldPosition);
    }

    private void SearchEnemy()
    {
        tr = Game.ActiveScene.Trace.Ray(new Ray(Replicant.GetEye().WorldPosition, Replicant.GetEye().WorldRotation.Forward), Replicant.AttackDistance)
        .IgnoreGameObject(Replicant.GameObject)
        .Run();

        if (!tr.Hit) return;

        var targetObject = tr.GameObject;

        if (targetObject.Tags.Has("enemy"))
        {
            var enemy = targetObject.Components.Get<EnemyBase>();

            if (enemy != null)
            {
                Replicant.replicantFSM.SetState<HandleAttackEnemy>();
            }
        }
    }

    private void CheckTargetObjectIsDead()
    {
       var target = Replicant.GetTargetObject();
        
        var enemy = target?.Components.Get<EnemyBase>();

        if (enemy == null)     
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