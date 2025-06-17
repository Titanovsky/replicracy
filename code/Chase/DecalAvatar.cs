using Sandbox;

public sealed class DecalAvatar : Component
{
    [Property] public string SteamId { get; set; }
    [RequireComponent] private Decal AvatarDecal { get; set; }

    protected override void OnStart()
    {
        if (SteamId != null)
            DrawAvatarDecal(SteamId);
    }

    public void DrawAvatarDecal(string SteamId)
    {
        AvatarDecal = GameObject.GetComponent<Decal>();

        AvatarDecal.ColorTexture = Texture.LoadAvatar(SteamId);
        AvatarDecal.SortLayer = 255;
    }
}
