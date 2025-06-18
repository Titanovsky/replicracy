using Sandbox;

public sealed class Building : Component
{
    [Property] private int Health { get; set; } = 100;
    [Property] public Color Color { get; set; } = Color.Red;
    public bool IsDead { get; set; }

    private int MaxHealth { get; set; }

    private MeshComponent MeshComponent { get; set; }
    private ModelRenderer ModelRenderer { get; set; }

    protected override void OnStart()
    {
        MaxHealth = Health;

        GetModelComponent();

    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        Health -= damage;

        float t = (MaxHealth - Health) / (float)MaxHealth;

        if (MeshComponent != null)
            MeshComponent.Color = MeshComponent.Color.LerpTo(Color, t);

        if (ModelRenderer != null)
            ModelRenderer.Tint = ModelRenderer.Tint.LerpTo(Color, t);

        CheckDead();
    }

    private void CheckDead()
    {
        if (Health <= 0)
        {
            IsDead = true;
        }
    }

    private void GetModelComponent()
    {
        MeshComponent = Components.Get<MeshComponent>();

        if (MeshComponent == null)
            ModelRenderer = Components.Get<ModelRenderer>();
    }
}
