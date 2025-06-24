using System;

public sealed class Cutscene : Component
{
    [Property, Feature("Info")] public string Name { get; set; } = string.Empty;
    [Property, Feature("Info")] public string Description { get; set; } = "This is description";

    [Property, Feature("Stats")] public List<CutscenePoint> Points { get; set; } = new();
    [Property, Feature("Stats")] public GameObject Obj { get; set; } //todo remove
    [Property, Feature("Stats")] public Action OnPlay { get; set; }
    [Property, Feature("Stats")] public Action OnFinish { get; set; }
    public bool IsPlaying { get; private set; } = false;
    private int _currentPointIndex = -1;
    private TimeUntil _delayMovingPoint;

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
        _currentPointIndex = -1;

        //todo remove godmode player, new black screen and return camera to player

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
        if (Points.Count == 0) { Finish(); return; }
        if (!_delayMovingPoint) return;

        var point = GetPoint();

        _delayMovingPoint = point.Delay;

        Obj.WorldPosition = point.WorldPosition;
        Obj.WorldRotation = point.WorldRotation;

        if (point == GetLastPoint())
            Finish();
    }

    private CutscenePoint GetPoint()
    {
        var index = (_currentPointIndex == -1) ? 0 : _currentPointIndex + 1;
        if (index >= Points.Count) return Points[0];

        return Points[index];
    }

    private CutscenePoint GetLastPoint()
    {
        return Points[Points.Count - 1];
    }

    protected override void OnFixedUpdate()
    {
        Move();
    }
}