public class LevelManager : Component
{
    public static LevelManager Instance { get; private set; }

    [Property] public LevelBase CurrentLevel { get; private set; }

    private string _sceneCredits = "scenes/credits.scene";

    public void Finish(bool force = false)
    {
        if (!CurrentLevel.IsValid()) return;
        if (!force && !CurrentLevel.CheckFinish()) return;

        if (CurrentLevel.NextLevelScene is not null)
            Scene.Load(CurrentLevel.NextLevelScene);
        else
            Scene.LoadFromFile(_sceneCredits);
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