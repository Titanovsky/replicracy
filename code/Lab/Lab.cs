public sealed class Lab : Component
{
    [Property, Feature("Props")] public GameObject Replicant { get; set; }

    [Property, Feature("Props"), Group("Buttons")] public GameObject ButtonBuy { get; set; }
    [Property, Feature("Props"), Group("Buttons")] public List<GameObject> ButtonsCancel { get; set; } = new();

    private void Prepare()
    {

    }

    protected override void OnStart()
    {
        Prepare();
    }

	//protected override void OnFixedUpdate()
	//{

	//}
}
