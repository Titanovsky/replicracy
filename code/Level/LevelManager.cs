public class LevelManager : Component
{
    public static LevelManager Instance { get; private set; }

    public LevelBase CurrentLevel { get; private set; }

    public void Finish(bool force = false)
    {

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

    private void Prepare()
    {
        //
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