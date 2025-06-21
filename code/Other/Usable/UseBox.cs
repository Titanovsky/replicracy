using Sandbox;

public sealed class UseBox : Component, IUsable
{
    [Property] public string UsableText { get; set; }
    [Property] public bool IsUsed { get; set; } = false;
    [Property] ModelRenderer boxModel { get; set; }
    [Property] HighlightOutline outline { get; set; }

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

        IsUsed = !IsUsed;

        ChangeColor();
    }

    private void ChangeColor()
    {
        if (IsUsed)
            boxModel.Tint = Color.Red;
        else
            boxModel.Tint = Color.White;
    }

    public string GetUsableText() => UsableText;
}
