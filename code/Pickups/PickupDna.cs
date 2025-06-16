using Sandbox;
using System;

public sealed class PickupDna : Component, Component.ITriggerListener
{
    [Property] public float Frequency { get; set; } = 5.43f;
    [Property] public float Amplitude { get; set; } = 6f;
    [Property] public float SpeedRotate { get; set; } = .25f;
    [Property] public int Dna { get; set; } = 2;

    private void PreparePickupManager()
    {
        var manager = PickupManager.Instance;
        if (manager == null) return;

        manager.Add(this);
    }

    protected override void OnStart()
    {
        PreparePickupManager();
    }

    public void OnTriggerEnter(Collider other)
    {
        var ply = other.Components.GetInAncestorsOrSelf<Player>();
        if (!ply.IsValid()) return;

        ply.Dna += Dna;

        DestroyGameObject();
    }
}
