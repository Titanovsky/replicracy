using Sandbox;
using System;

public sealed class PickupManager : Component
{
    [Property] public List<PickupDna> Pickups { get; set; } = new();

    private List<Vector3> _cachePos = new();

    private void Prepare()
    {
        foreach (var pickup in Pickups)
        {
            if (!pickup.IsValid()) continue;

            _cachePos.Add(pickup.WorldPosition);
        }
    }

    private void MovingAndRotating()
    {
        for (int i = 0; i < Pickups.Count; i++)
        {
            var pickup = Pickups[i];
            if (!pickup.IsValid()) continue;

            pickup.WorldRotation *= new Angles(0, 1f * pickup.SpeedRotate, 0);
            pickup.WorldPosition = _cachePos[i] + new Vector3(1f, 1f, MathF.Sin(Time.Now * pickup.Frequency) * pickup.Amplitude);
        }
    }

    protected override void OnStart()
    {
        Prepare();
    }

    protected override void OnFixedUpdate()
    {
        MovingAndRotating();
    }
}