using Sandbox;
using System;

public sealed class UseBox : Component, IUsable
{
    [Property, TextArea] public string UsableText { get; set; }
    [Property] public bool IsUsed { get; set; } = false;
    [Property] public float Delay { get; set; } = .75f;
    [Property] ModelRenderer boxModel { get; set; }
    [Property] HighlightOutline outline { get; set; }

    [Property] public Action OnCallback { get; set; }

    private TimeUntil _delay;

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
        if (!_delay) return;
        _delay = Delay;

        IsUsed = !IsUsed;

        ChangeColor();

        OnCallback?.Invoke();
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
