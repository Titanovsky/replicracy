using Sandbox;

public sealed class LabReplicant : Component
{
    [Property] public float SpeedMove { get; set; } = 5f;

    protected override void OnFixedUpdate()
	{
        WorldRotation *= Rotation.FromYaw(SpeedMove);
	}
}
