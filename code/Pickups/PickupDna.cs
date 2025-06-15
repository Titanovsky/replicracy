using Sandbox;
using System;

public sealed class PickupDna : Component
{
    [Property] public float Frequency { get; set; } = 5.43f;
    [Property] public float Amplitude { get; set; } = 6f;
    [Property] public float SpeedRotate { get; set; } = .25f;

    private Vector3 _cachePos = Vector3.Zero;

    private void Prepare()
    {
        _cachePos = WorldPosition;
    }

    private void Rotating()
    {
        WorldRotation *= new Angles(0, 1f * SpeedRotate, 0);
    }

    private void Moving()
    {
        WorldPosition = _cachePos + new Vector3(1f, 1f, MathF.Sin(Time.Now * Frequency) * Amplitude);
    }

    protected override void OnStart()
    {
        Prepare();
    }

    protected override void OnUpdate()
    {
        Rotating();
        Moving();
    }
}
