public class ReturnToPlayer : MovableState
{
    public ReturnToPlayer(Replicant replicant) : base(replicant) { }

    private Vector3 _returnPoint;
    private Vector3 oldPlayerPos;
    private float _animWorkaround = .5f; //! fuck anim move_x and move_y

    public override void Enter()
    {
        oldPlayerPos = Vector3.Zero;
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

    public override void Update()
    {
        var playerPosition = Player.Instance.GameObject.WorldPosition;
        UpdatedRotation(playerPosition);

        if (oldPlayerPos == playerPosition) return;

        _returnPoint = (Vector3)Game.ActiveScene.NavMesh.GetRandomPoint(playerPosition, 300);

        oldPlayerPos = playerPosition;

        Replicant.MoveToPoint(_returnPoint);

        AnimationPlay();
    }
}