using System;

public sealed class Cutscene : Component
{
    [Property, Feature("Info")] public string Name { get; set; } = string.Empty;
    [Property, Feature("Info")] public string Description { get; set; } = "This is description";

    [Property, Feature("Stats")] public List<CutscenePoint> Points { get; set; } = new();
    [Property, Feature("Stats")] public Action OnPlay { get; set; }
    [Property, Feature("Stats")] public Action OnFinish { get; set; }
    public bool IsPlaying { get; set; } = false;

    public void Play()
    {
        if (IsPlaying) return;

        PreparePlay();

        IsPlaying = true;

        OnPlay?.Invoke();
    }

    public void Finish()
    {
        if (!IsPlaying) return;

        IsPlaying = false;

        OnFinish?.Invoke();
    }

    private void PreparePlay()
    {
        //todo godmode player
        //todo black screen
        //todo prepare camera
    }

    private void Move()
    {
        if (!IsPlaying) return;
    }

    protected override void OnFixedUpdate()
    {
        Move();
    }
}