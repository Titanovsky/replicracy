using Sandbox;
using System.Threading.Tasks;

public sealed class Bullet : Component, Component.ITriggerListener
{
    [Property] public float MyProperty { get; set; } = 10;

    private void Prepare()
	{
        Log.Info($"Prepare {GameObject}");
        DieDelay(2f).Start();
    }

    private async Task DieDelay(float delay)
    {
        Log.Info($"Remove {GameObject}");
        await Task.DelaySeconds(delay);
        //if (!this.IsValid()) return;

        Log.Info($"Remove {GameObject}");
        DestroyGameObject();
    }

	private void Move()
	{

	}

    protected override void OnStart()
    {
        Log.Info($"Remove {GameObject}");
        Prepare();
    }

    protected override void OnFixedUpdate()
	{
		Move();
        Log.Info($"Remove {GameObject}");
    }

    public void OnTriggerEnter(Collider other)
    {
        Destroy();
    }
}
