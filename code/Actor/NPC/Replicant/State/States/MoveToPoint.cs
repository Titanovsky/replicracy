public class MoveToPoint : MovableState
{
    private int randomRadius = 150;

    public MoveToPoint(Replicant replicant) : base(replicant) { }

    public override void Enter()
    {
        if (Replicant.GetTargetPoint() == Vector3.Zero)
        {
            Replicant.replicantFSM.SetState<Idle>();
            return;
        }

        var randomPoint = (Vector3)Game.ActiveScene.NavMesh.GetRandomPoint(Replicant.GetTargetPoint(), randomRadius);

        Replicant.MoveToPoint(randomPoint);
    }

    public override void Update()
    {
        UpdatedRotation(Replicant.GetTargetPoint());
    }
}