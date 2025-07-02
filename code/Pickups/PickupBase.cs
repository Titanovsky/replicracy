public abstract class PickupBase : Component, Component.ITriggerListener
{
    [Property] public virtual float Frequency { get; set; } = 5.43f;
    [Property] public virtual float Amplitude { get; set; } = 6f;
    [Property] public virtual float SpeedRotate { get; set; } = .25f;

    public virtual void OnTouch(Collider other)
    {
    }

    public void OnTriggerEnter(Collider other)
    {
        OnTouch(other);
    }
}