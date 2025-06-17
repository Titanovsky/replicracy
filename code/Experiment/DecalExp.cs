using Sandbox;

public sealed class DecalExp : Component
{
	protected override void OnStart()
	{
        var decal = AddComponent<Decal>();
        decal.ColorTexture = Texture.LoadAvatar(76561198086321085);
        decal.SortLayer = 255;
    }
}
