using Sandbox;
using System;

public sealed class UseColorMiniGameButton : Component, IUsable
{
    [Property] public string UsableText { get; set; }
    [Property] public int ButtonId { get; set; }
    [Property] public PointLight ButtonLight { get; set; }

    private float activatorDelay = 0.5f;

    public bool IsUsed { get; set; }

    private RealTimeUntil _activeLightTimer;
    private RealTimeUntil _activatorTimer;

    public Action<int> OnUseCallback { get; set; }

    protected override void OnUpdate()
    {
        CheckLightTimer();
    }

    public void Use()
    {
        if (!_activatorTimer) return;

        TurnOnLight(0.4f);
        OnUseCallback?.Invoke(ButtonId);

        _activatorTimer = activatorDelay;
    }

    public void TurnOnLight(float timer)
    {
        ButtonLight.Enabled = true;

        _activeLightTimer = timer;
    }

    public void CheckLightTimer()
    {
        if (!ButtonLight.Enabled) return;

        if (!_activeLightTimer) return;

        ButtonLight.Enabled = false;
    }

    public void EnableHightlight() { }

    public void DisableHightlight() { }

    public string GetUsableText() => UsableText;
}
