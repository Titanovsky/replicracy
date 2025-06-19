using Sandbox;

public sealed class UseBox : Component, IUsable
{
    [Property] ModelRenderer boxModel { get; set; }
    [Property] HighlightOutline outline {  get; set; }

    public void EnableHightlight()
    {
        outline.Color = outline.Color.WithAlpha(1);
    }

    public void DisableHightlight()
    {
        outline.Color = outline.Color.WithAlpha(0);
    }

    public void Use()
    {
        if (boxModel == null) return;

        if (boxModel.Tint == Color.Red)
            boxModel.Tint = Color.White;
        else
            boxModel.Tint = Color.Red;
    }
}
