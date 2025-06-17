using Sandbox;

public sealed class DecalExp : Component
{
	protected override void OnStart()
	{
        var decal = AddComponent<Decal>();
        decal.ColorTexture = Texture.LoadAvatar(76561197996859119);
        decal.SortLayer = 255;
    }
}
