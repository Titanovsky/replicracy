using Sandbox.Services;

public sealed class PickupSecret : PickupBase
{
    [Property] public override float Frequency { get; set; } = 5.43f;
    [Property] public override float Amplitude { get; set; } = 6f;
    [Property] public override float SpeedRotate { get; set; } = .25f;

    [Property] public string Name { get; set; } = "default";
    [Property] public SoundEvent PickupSound { get; set; }

    private void PreparePickupManager()
    {
        var manager = PickupManager.Instance;
        if (manager == null) return;

        manager.Add(this);
    }

    private void SetAchiv()
    {
        var ply = Player.Instance;
        if (!ply.CanAchievement()) return;

        Achievements.Unlock("first_secret");

        if (ply.CollectSecrets >= LevelManager.Instance.CurrentLevel.MaxSecrets)
            Achievements.Unlock($"secret_{Name}");
    }

    protected override void OnStart()
    {
        PreparePickupManager();
    }

    public override void OnTouch(Collider other)
    {
        var ply = other.Components.GetInAncestorsOrSelf<Player>();
        if (!ply.IsValid()) return;

        ply.CollectSecrets += 1;
        ply.SecretsHud.Show();

        PlaySound();
        SetAchiv();

        DestroyGameObject();
    }

    private void PlaySound()
    {
        if (PickupSound != null)
            Sound.Play(PickupSound, WorldPosition);
    }
}
