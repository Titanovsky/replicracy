using Sandbox;

public sealed class UseBox : Component, IUsable
{
    [Property] ModelRenderer boxModel;

    public void Use()
    {
        if (boxModel == null) return;

        if (boxModel.Tint == Color.Red)
            boxModel.Tint = Color.White;
        else
            boxModel.Tint = Color.Red;
    }
}
