using Sandbox;

public sealed class Loot : Component
{
    [Property] private int Health { get; set; } = 100;
    [Property] private int DNA { get; set; } = 5;
    [Property] public Color DamageColor { get; set; } = Color.Red;
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

        LerpColor();

        CheckDead();
    }

    private void CheckDead()
    {
        if (Health <= 0)
        {
            IsDead = true;

            Player.Instance.CollectDna += DNA;
            Player.Instance.HeaderLevel.Show();
        }
    }

    private void GetModelComponent()
    {
        MeshComponent = Components.Get<MeshComponent>();

        if (MeshComponent == null)
            ModelRenderer = Components.Get<ModelRenderer>();
    }

    private void LerpColor()
    {
        float t = (MaxHealth - Health) / (float)MaxHealth;

        if (MeshComponent != null)
            MeshComponent.Color = MeshComponent.Color.LerpTo(DamageColor, t);

        if (ModelRenderer != null)
            ModelRenderer.Tint = ModelRenderer.Tint.LerpTo(DamageColor, t);
    }
}
