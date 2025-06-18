public abstract class PickupBase : Component, Component.ITriggerListener
{
    [Property] public float Frequency { get; set; } = 5.43f;
    [Property] public float Amplitude { get; set; } = 6f;
    [Property] public float SpeedRotate { get; set; } = .25f;
}