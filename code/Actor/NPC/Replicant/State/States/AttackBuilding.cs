public class AttackBuilding : MovableState
{
    public AttackBuilding(Replicant replicant) : base(replicant) { }

    private SceneTraceResult tr;

    private Loot attackLoot;

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

        SearchLoot();

        Attack();
    }

    public override void Exit()
    {
        DeletTargerLoot();
    }

    private void Attack()
    {
        if (attackLoot == null) return;

        if (!Replicant.IsAttackAllowed()) return;

        if (attackLoot.IsDead)
        {
            DeletTargerLoot();
            Replicant.replicantFSM.SetState<ReturnToPlayer>();
            return;
        }

        attackLoot.TakeDamage(Replicant.AttackDamage);

        Replicant.ReseAttackTimer();
    }

    private void SearchLoot()
    {
        if (attackLoot != null) return;

        tr = Game.ActiveScene.Trace.Ray(new Ray(Replicant.GetEye().WorldPosition, Replicant.GetEye().WorldRotation.Forward), Replicant.AttackDistance)
        .IgnoreGameObject(Replicant.GameObject)
        .Run();

        if (!tr.Hit) return;

        var targetObject = tr.GameObject;

        if (targetObject.Tags.Has("building"))
        {
            var loot = targetObject.Components.Get<Loot>();

            if (loot != null && !loot.IsDead)
            {
                attackLoot = loot;

                Replicant.HideReplicant();
                Replicant.HealthBar.IsVisible = false;
            }
            else
                Replicant.replicantFSM.SetState<ReturnToPlayer>();
        }
    }

    private void DeletTargerLoot()
    {
        attackLoot = null;
        Replicant.ShowReplicant();
        Replicant.HealthBar.IsVisible = true;
    }
}