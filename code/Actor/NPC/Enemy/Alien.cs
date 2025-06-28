using Sandbox;

public sealed class Alien : EnemyBase
{
    [Property] public SkinnedModelRenderer Renderer { get; set; }
    [Property] public NavMeshAgent NavMeshAgent { get; set; }
    [Property] public float SearchRadius { get; set; } = 500f;
    [Property] public float LostRadius { get; set; } = 700f;
    [Property] public float MovingDelay { get; set; } = 5f;
    [Property] public float MovingStartPosRadius { get; set; } = 150f;

    [Property][Category("Weapon")] public float AttackDamage { get; set; } = 10f;
    [Property][Category("Weapon")] public float AttackDelay { get; set; } = 6f;
    [Property][Category("Weapon")] public float AttackDistance { get; set; } = 200f;
    [Property][Category("Weapon")] public SoundEvent AttackSound { get; set; }
    [Property][Category("Weapon")] public GameObject AttackPosition { get; set; }
    [Property][Category("Weapon")] public GameObject ProjectilePrefab { get; set; }

    protected override void OnUpdate()
	{

	}
}
