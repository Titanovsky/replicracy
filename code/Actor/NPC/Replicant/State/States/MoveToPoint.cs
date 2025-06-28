public class MoveToPoint : MovableState
{
    private int randomRadius = 150;
    private float _animWorkaround = .5f; //! fuck anim move_x and move_y

    private Vector3 randomPoint;

    public MoveToPoint(Replicant replicant) : base(replicant) { }

    public override void Enter()
    {
        if (Replicant.GetTargetPoint() == Vector3.Zero)
        {
            Replicant.replicantFSM.SetState<Idle>();
            return;
        }

        var a = Game.ActiveScene.NavMesh.GetRandomPoint(Replicant.GetTargetPoint(), randomRadius);
        if (a.HasValue)
        {
            randomPoint = Game.ActiveScene.NavMesh.GetRandomPoint(Replicant.GetTargetPoint(), randomRadius).Value;

            Replicant.MoveToPoint(randomPoint);
        }
    }

    public override void Update()
    {
        AnimationPlay();
        UpdatedRotation(Replicant.GetTargetPoint());

        CheckPointDistance();
    }

    private void CheckPointDistance()
    {
        var distance = Replicant.WorldPosition.Distance(randomPoint);

        if (distance < 100)
        {
            Replicant.replicantFSM.SetState<Idle>();
        }
    }

    private void AnimationPlay()
    {
        var dir = Replicant.Agent.Velocity;
        var forward = Replicant.WorldRotation.Forward.Dot(dir);
        var sideward = Replicant.WorldRotation.Right.Dot(dir);

        //todo aim_head
        //Replicant.Renderer.Set("aim_head", (forward - _targetPos).Normal);
        Replicant.Renderer.Set("move_y", sideward * _animWorkaround);
        Replicant.Renderer.Set("move_x", forward * _animWorkaround);
    }
}