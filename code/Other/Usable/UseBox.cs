using Sandbox;

public sealed class UseBox : Component, IUsable
{
    [Property, TextArea] public string UsableText { get; set; }
    [Property] public bool IsUsed { get; set; } = false;
    [Property] ModelRenderer boxModel { get; set; }
    [Property] HighlightOutline outline { get; set; }

    public void EnableHightlight()
    {
        if (!outline.IsValid()) return;

        outline.Color = outline.Color.WithAlpha(1);
    }

    public void DisableHightlight()
    {
        if (!outline.IsValid()) return;

        outline.Color = outline.Color.WithAlpha(0);
    }

    public void Use()
    {
        IsUsed = !IsUsed;

        ChangeColor();
    }

    private void ChangeColor()
    {
        if (!boxModel.IsValid()) return;

        if (IsUsed)
            boxModel.Tint = Color.Red;
        else
            boxModel.Tint = Color.White;
    }

    public string GetUsableText() => UsableText;
}
