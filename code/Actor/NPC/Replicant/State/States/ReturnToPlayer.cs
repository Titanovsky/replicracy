public class ReturnToPlayer : MovableState
{
    public ReturnToPlayer(Replicant replicant) : base(replicant) { }

    private Vector3 _returnPoint;
    private Vector3 oldPlayerPos;

    public override void Enter()
    {
        oldPlayerPos = Vector3.Zero;
    }

    public override void Update()
    {
        var playerPosition = Player.Instance.GameObject.WorldPosition;
        UpdatedRotation(playerPosition);

        if (oldPlayerPos == playerPosition) return;

        _returnPoint = (Vector3)Game.ActiveScene.NavMesh.GetRandomPoint(playerPosition, 300);

        oldPlayerPos = playerPosition;

        Replicant.MoveToPoint(_returnPoint);
    }
}