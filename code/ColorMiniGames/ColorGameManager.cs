using Sandbox;
using System;

public sealed class ColorGameManager : Component
{
    [Property][Category("Game Param")] public float StartDelayChangeColor { get; set; } = 1f;
    [Property][Category("Game Param")] public int StartCountAnsers { get; set; } = 5;
    [Property][Category("Game Param")] public float WaitingAnswerDelay { get; set; } = 15f;
    [Property][Category("Game Param")] public float RoundCount { get; set; } = 3f;
    [Property][Category("Buttons")] public List<UseColorMiniGameButton> PlayingButtons { get; set; }
    [Property][Category("CallBack")] public Action OnSuccessFinished { get; set; }

    private int[] _answers;
    
    private float _currentRound { get; set; } = 1f;
    private bool _isPlaying = false;

    private bool _isFinished { get; set; } = false;

    private RealTimeUntil _changeColorTimer { get; set; }
    private RealTimeUntil _waitingAnswerTimer { get; set; }

    private Random rnd;

    protected override void OnStart()
    {
        rnd = new();

        SubscribeToButtons();
    }

    protected override void OnUpdate()
    {
        CheckWaitingAnswerTimer();
    }

    protected override void OnDestroy()
    {
        UnSubscribeToButtons();
    }

    private void CheckWaitingAnswerTimer()
    {
        if (!_isPlaying) return;

        if (!_waitingAnswerTimer) return;

        FailedRound();
    }

    private void StartGame()
    {

    }

    private void CheckButtonCallback(int answer)
    {
        Log.Info(answer);

        if (!_isPlaying) return;

       

    }

    private void GenerateRoundAnsers()
    {
        _answers = null;

        int buttonCount = PlayingButtons.Count;

        _answers = new int[StartCountAnsers];

        for (int i = 0; i < StartCountAnsers; i++)
        {
            _answers[i] = rnd.Next(0, buttonCount);
        }
    }

    private void FailedRound()
    {
         _isPlaying = false;
        _currentRound = 1f;
    }
    
    private void SuccessGame()
    {
         _isPlaying = false;
        _currentRound = 1f;

        OnSuccessFinished?.Invoke();
    }

    private void SubscribeToButtons()
    {
        if (PlayingButtons.Count == 0) return;

        foreach (var button in PlayingButtons)
        {
            if (button != null)
                button.OnUseCallback += CheckButtonCallback;
        }
    }

    private void UnSubscribeToButtons()
    {
        if (PlayingButtons.Count == 0) return;

        foreach (var button in PlayingButtons)
        {
            if (button != null)
                button.OnUseCallback -= CheckButtonCallback;
        }
    }

}
