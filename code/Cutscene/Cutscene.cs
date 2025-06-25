using System;

public sealed class Cutscene : Component
{
    [Property, Feature("Info")] public string Name { get; set; } = string.Empty;
    [Property, Feature("Info")] public string Description { get; set; } = "This is description";

    [Property, Feature("Stats")] public List<CutscenePoint> Points { get; set; } = new();
    [Property, Feature("Stats")] public GameObject Obj { get; set; } //todo remove after development
    [Property, Feature("Stats")] public Action OnPlay { get; set; }
    [Property, Feature("Stats")] public Action OnFinish { get; set; }
    public bool IsPlaying { get; private set; } = false;
    private int _currentPointIndex = -1;
    private TimeUntil _delayMovingPoint;

    private Transform _objStartTransform; //todo remove after development 

    //todo remove after development
    private void StartTest()
    {
        Play();
    }

    public void Play()
    {
        if (IsPlaying) return;
        if (Points.Count == 0) return;

        Log.Info($"[Cutscene] Start {Name}");

        PreparePlay();

        _objStartTransform = Obj.WorldTransform;

        IsPlaying = true;

        OnPlay?.Invoke();
    }

    public void Finish()
    {
        if (!IsPlaying) return;

        IsPlaying = false;
        _currentPointIndex = -1;

        //todo remove godmode player, new black screen and return camera to player

        Obj.WorldTransform = _objStartTransform;

        Log.Info($"[Cutscene] Finish {Name}");

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
        if (!_delayMovingPoint) return;

        if (_currentPointIndex == Points.Count - 1) { Finish(); return; }

        var point = GetPoint();

        _delayMovingPoint = point.Delay;

        Obj.WorldPosition = point.WorldPosition;
        Obj.WorldRotation = point.WorldRotation;

        Log.Info($"[Cutscene] Moving {Name} {point.GameObject}");
    }

    private CutscenePoint GetPoint()
    {
        var index = (_currentPointIndex == -1) ? 0 : _currentPointIndex + 1;
        _currentPointIndex = index;

        if (index >= Points.Count) return Points[0];

        Log.Info($"{index}");

        return Points[index];
    }

    protected override void OnStart()
    {
        StartTest();
    }

    protected override void OnFixedUpdate()
    {
        Move();
    }
}