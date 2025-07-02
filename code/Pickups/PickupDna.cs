public sealed class PickupDna : PickupBase
{
    [Property] public override float Frequency { get; set; } = 5.43f;
    [Property] public override float Amplitude { get; set; } = 6f;
    [Property] public override float SpeedRotate { get; set; } = .25f;

    [Property] public int Dna { get; set; } = 2;
    [Property] public SoundEvent PickupSound { get; set; }

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

    public override void OnTouch(Collider other)
    {
        var ply = other.Components.GetInAncestorsOrSelf<Player>();
        if (!ply.IsValid()) return;

        ply.Dna += Dna;
        ply.CollectDna += 1;
        ply.HeaderLevel.Show();

        PlaySound();

        DestroyGameObject();
    }

    private void PlaySound()
    {
        if (PickupSound != null)
            Sound.Play(PickupSound, WorldPosition);
    }
}
