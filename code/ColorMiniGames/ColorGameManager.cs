using Sandbox;
using System;
using System.Threading.Tasks;

public sealed class ColorGameManager : Component
{
    [Property][Category("Game Param")] public float StartDelayChangeColor { get; set; } = 1f;
    [Property][Category("Game Param")] public int StartCountAnswers { get; set; } = 3;
    [Property][Category("Game Param")] public float WaitingAnswerDelay { get; set; } = 10f;
    [Property][Category("Game Param")] public float RoundCount { get; set; } = 3f;

    [Property][Category("Buttons")] public List<UseColorMiniGameButton> PlayingButtons { get; set; }

    [Property] [Category("Sounds")] private SoundEvent GameStarted { get; set; }
    [Property] [Category("Sounds")] private SoundEvent FailedGame { get; set; }
    [Property] [Category("Sounds")] private SoundEvent SuccesRound { get; set; }
    [Property] [Category("Sounds")] private SoundEvent ButtonClick { get; set; }

    [Property][Category("CallBack")] public Action OnSuccessFinished { get; set; }

    private int[] _answers;
    private int[] _playerAnswers;

    private bool _isPlaying = false;
    private bool _isRoundStarted = false;
    private bool _isFinished  = false;

    [Property] private float _currentRound { get; set; } = 0;
    [Property] private int _currentAnswer = 0;
    [Property] private int _countRoundAnswer;

    private RealTimeUntil _waitingAnswerTimer { get; set; }

    private Random rnd;

    protected override void OnStart()
    {
        _countRoundAnswer = StartCountAnswers;

        rnd = new();

        SubscribeToButtons();
    }

    protected override void OnUpdate()
    {
        CheckWaitingAnswerTimer();

        CheckAnsers();
    }

    protected override void OnDestroy()
    {
        UnSubscribeToButtons();
    }

    private void CheckWaitingAnswerTimer()
    {
        if (!_isRoundStarted) return;
        if (!_waitingAnswerTimer) return;

        FailedRound();
    }

    private void CheckAnsers()
    {
        if (!_isRoundStarted) return;
        if (_currentAnswer != _answers.Length) return;

        for (int i = 0; i < _answers.Length; i++)
        {
            if (_answers[i] != _playerAnswers[i])
            {
                FailedRound(); return;
            }
        }

        if (_currentRound == RoundCount)
            SuccessGame();
        else
        {
            Sound.Play(SuccesRound);
            _= NextRound();
        }
    }

    private void StartGame()
    {
        Sound.Play(GameStarted);

        _isPlaying = true;

        _ = NextRound();
    }

    private void CheckButtonCallback(int answer)
    {
        if (_isFinished) return;

        Sound.Play(ButtonClick);

        if (!_isPlaying)
        {
            StartGame();
            return;
        };

        _playerAnswers[_currentAnswer] = answer;
        _currentAnswer++;

        ResetWaitingTimer();
    }

    private async Task NextRound()
    {
        _isRoundStarted = false;

        _currentRound++;
        _currentAnswer = 0;

        if (_currentRound > 1)
            _countRoundAnswer++;

        EnabledButtons(false);
        GenerateRoundAnswers();

        _playerAnswers = new int[_answers.Length];

        await ShowColor();

        EnabledButtons(true);
        ResetWaitingTimer();

        _isRoundStarted = true;
    }

    private void GenerateRoundAnswers()
    {
        _answers = null;

        int buttonCount = PlayingButtons.Count;

        _answers = new int[_countRoundAnswer];

        for (int i = 0; i < _countRoundAnswer; i++)
        {
            _answers[i] = rnd.Next(0, buttonCount);
        }
    }

    private async Task ShowColor()
    {
        await Task.DelaySeconds(StartDelayChangeColor + 1);

        foreach (int i in _answers)
        {
            PlayingButtons[i].TurnOnLight(StartDelayChangeColor);

            await Task.DelaySeconds(StartDelayChangeColor + 1);
        }
    }

    private void FailedRound()
    {
        GameStop();

        Sound.Play(FailedGame);
    }

    private void SuccessGame()
    {
        GameStop();

        Sound.Play(SuccesRound);

        _isFinished = true;

        OnSuccessFinished?.Invoke();
    }

    private void GameStop()
    {
        _isRoundStarted = false;
        _currentRound = 0;
        _currentAnswer = 0;
        _countRoundAnswer = StartCountAnswers;
    }

    private void EnabledButtons(bool isEnable)
    {
        foreach (var button in PlayingButtons)
        {
            if (button != null)
            {
                button.IsButtonActive = isEnable;
            }
        }
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

    public void ResetWaitingTimer() => _waitingAnswerTimer = WaitingAnswerDelay;
}
