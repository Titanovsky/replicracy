public class Idle : ReplicantState
{
    public Idle(Replicant replicant) : base(replicant) { }

    public override void Update()
    {
        CheckDistanceToPlayer();
    }

    private void CheckDistanceToPlayer()
    {
        var playerPosition = Player.Instance.GameObject.WorldPosition;
        var distance = Replicant.WorldPosition.Distance(playerPosition);

        if (distance > Replicant.MaxDistanceToPlayer)
        {
            Replicant.replicantFSM.SetState<ReturnToPlayer>();
        }
    }
}