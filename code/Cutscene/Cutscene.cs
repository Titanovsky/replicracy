using System;
using System.Threading.Tasks;

public sealed class Cutscene : Component
{
    [Property, Feature("Info")] public string Name { get; set; } = string.Empty;
    [Property, Feature("Info")] public string Description { get; set; } = "This is description";

    [Property, Feature("Stats")] public List<CutscenePoint> Points { get; set; } = new();
    [Property, Feature("Stats")] public Action OnPlay { get; set; }
    [Property, Feature("Stats")] public Action OnFinish { get; set; }

    public bool IsPlaying { get; private set; } = false;
    private int _currentPointIndex = -1;
    private TimeUntil _delayMovingPoint;

    private Player _player;
    private CameraComponent _playerCamera;
    private CutscenePoint _currentPoint;

    private bool IsAttainCurrentPoint = false;

    [Button("Start Test Scene")]
    private void StartTest()
    {
        _ = Play();
    }

    public async Task Play()
    {
        if (IsPlaying) return;
        if (Points.Count == 0) return;

        Log.Info($"[Cutscene] Start {Name}");

        await PreparePlay();
        await SetStartPosition();

        IsPlaying = true;

        OnPlay?.Invoke();
    }

    public async Task Finish()
    {
        if (!IsPlaying) return;

        IsPlaying = false;
        _currentPointIndex = -1;

        await InFadePlayer();

        SetUsePlayerCameraControl(true);
        _playerCamera = null;

        await OutFadePlayer();

        Log.Info($"[Cutscene] Finish {Name}");

        OnFinish?.Invoke();
    }

    private async Task PreparePlay()
    {
        //todo godmode player

        await InFadePlayer();

        _playerCamera = Scene.Camera;

        SetUsePlayerCameraControl(false);
    }

    private async Task SetStartPosition()
    {
        ChangeCurrentPoint();

        _playerCamera.WorldPosition = _currentPoint.WorldPosition;
        _playerCamera.WorldRotation = _currentPoint.WorldRotation;

        await OutFadePlayer();
    }

    private void CheckPointPassed()
    {
        if (!IsPlaying) return;
        if (!IsAttainCurrentPoint) return;
        if (!_delayMovingPoint) return;

        ChangeCurrentPoint();
        IsAttainCurrentPoint = false;
    }

    private void MoveToPoint()
    {
        if (!IsPlaying) return;
        if (IsAttainCurrentPoint) return;
        if (!_delayMovingPoint) return;

        _playerCamera.WorldPosition = _playerCamera.WorldPosition
            .LerpTo(_currentPoint.WorldPosition, _currentPoint.SpeedToPoint * Time.Delta, true);

        var diff = _currentPoint.WorldPosition - _playerCamera.WorldPosition;

        if (diff.LengthSquared < 0.01f)
        {
            IsAttainCurrentPoint = true;

            _delayMovingPoint = _currentPoint.Delay;
        }
    }

    private void RotateToPoint()
    {
        if (!IsPlaying) return;
        if (IsAttainCurrentPoint) return;
        if (!_delayMovingPoint) return;

        var targetRotation = _currentPoint.WorldRotation;
        var currentRotation = _playerCamera.WorldRotation;
        var rotationDiff = Rotation.Difference(targetRotation, currentRotation);

        _playerCamera.WorldRotation = currentRotation
            .LerpTo(targetRotation, _currentPoint.SpeedToPoint * Time.Delta, true);
    }

    private void ChangeCurrentPoint()
    {
        var index = (_currentPointIndex == -1) ? 0 : _currentPointIndex + 1;
        _currentPointIndex = index;

        if (index >= Points.Count)
        {
            _ = Finish();
            return;
        }

        Log.Info($"{index}");

        _currentPoint = Points[index];
    }

    private void SetUsePlayerCameraControl(bool isControll) => _player.PlayerController.UseCameraControls = isControll;

    protected override void OnStart()
    {
        _player = Player.Instance;
    }

    protected override void OnDestroy()
    {
        _ = Finish();

        _player = null;
    }

    protected override void OnFixedUpdate()
    {
        CheckPointPassed();

        MoveToPoint();
        RotateToPoint();
    }

    private async Task InFadePlayer() => await _player.Fade.InFade(1, 1);
    private async Task OutFadePlayer() => await _player.Fade.OutFade(1);
}