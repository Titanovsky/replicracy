public class HandleAttackEnemy : ReplicantState
{
    public HandleAttackEnemy(Replicant replicant) : base(replicant) { }

    private SceneTraceResult tr;

    public override void Update()
    {
        AttackEnemy();
    }

    private void AttackEnemy()
    {
        tr = Game.ActiveScene.Trace.Ray(new Ray(Replicant.GetEye().WorldPosition, Replicant.GetEye().WorldRotation.Forward), 40)
        .IgnoreGameObject(Replicant.GameObject)
        .Run();

        if (!tr.Hit)
        {
            Replicant.replicantFSM.SetState<FollowToEnemy>();
            return;
        };

        var targetObject = tr.GameObject;

        if (targetObject.Tags.Has("enemy"))
        {
            var enemy = targetObject.Components.Get<EnemyBase>();

            if (enemy != null)
            {
                if (!Replicant.IsAttackAllowed()) return;

                enemy.OnDamage(new DamageInfo(
                    Replicant.AttackDamage, 
                    Replicant.GameObject, 
                    Replicant.GameObject));

                Replicant.ReseAttackTimer();
            }
        }
        else
            Replicant.replicantFSM.SetState<FollowToEnemy>();
    }

}