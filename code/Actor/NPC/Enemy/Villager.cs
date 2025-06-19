using Sandbox;

public sealed class Villager : EnemyBase
{
    protected override void OnStart()
    {
        GameObject.Name = $"😈 Enemy - {GameObject.Name}";
    }

	protected override void OnUpdate()
	{

	}

    public override void OnTriggerEnter(Collider other)
    {
        
    }

    public override void OnDamage(in DamageInfo damage)
    {

    }
}
