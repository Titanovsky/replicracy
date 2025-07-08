using Sandbox;

public sealed class Teleporter : Component, Component.ITriggerListener
{
    [Property] GameObject TeleportationPoint { get; set; }
    [Property] SoundEvent TeleportSound { get; set; }

    Collider TriggerTeleportCollider { get; set; }

    protected override void OnAwake()
    {
        TriggerTeleportCollider = Components.Get<Collider>();
    }

    public void OnTriggerEnter(Collider other)
    {
        var gameObject = other.GameObject.Parent;

        if (gameObject.Tags.Has("player"))
            TeleportPlayerReplicants();

        TeleportToPoint(gameObject);
    }

    public void TeleportToPoint(GameObject gameObject)
    {
        gameObject.WorldPosition = TeleportationPoint.WorldPosition;

        Sound.Play(TeleportSound, TeleportationPoint.WorldPosition);
    }

    private void TeleportPlayerReplicants()
    {
        var player = Player.Instance;

        if (player == null)
            return;

        var replicants = player.ReplicantController?.Replicants;

        foreach (var replicant in replicants)
        {
            if (replicant == null || !replicant.IsValid())
                continue;

            replicant.DisableAgent();

            replicant.GameObject.WorldPosition = TeleportationPoint.WorldPosition;

            replicant.EnabledAgent();
        }
    }
}
