using Sandbox;

public sealed class Player : Component
{
	public static Player Instance { get; private set; }

    [Property] public float MaxHealth { get; set; } = 0f;
    [Property] public float Health { get; set; } = 0f;
    [Property] public int Dna { get; set; } = 0;

	private void Prepare()
	{
		MaxHealth = 100f;
		Health = MaxHealth;
		Dna = 0;
	}

    private void CreateSingleton()
	{
		if (Instance is null)
			Instance = this;
    }

    protected override void OnStart()
    {
		Prepare();
    }

	protected override void OnAwake()
	{
		CreateSingleton();

    }
}
