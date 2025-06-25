public sealed class LabInfo : Component
{
    public static LabInfo Instance {  get; private set; }

    private void CreateSingleton()
    {
        if (Instance is null)
            Instance = this;
    }

    private void DestroySingleton()
    {
        Instance = null; //? maybe should be remove cuz Save
    }

    protected override void OnAwake()
    {
        CreateSingleton();
    }

    protected override void OnDestroy()
    {
        DestroySingleton();
    }
}