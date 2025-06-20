public class LevelManager : Component
{
    public static LevelManager Instance { get; private set; }

    [Property] public LevelBase CurrentLevel { get; private set; }

    public void Finish(bool force = false)
    {

    }

    public void Start()
    {
        if (!CurrentLevel.IsValid()) return;

        CurrentLevel.Act(1);
    }

    private void CreateSingleton()
    {
        if (Instance is null)
            Instance = this;
    }

    private void DestroySingleton()
    {
        Instance = null;
    }

    protected override void OnAwake()
    {
        CreateSingleton();
    }

    protected override void OnDestroy()
    {
        DestroySingleton();
    }

    protected override void OnStart()
    {
        Start();
    }
}