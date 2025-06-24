using System;

public class TriggerAct : TriggerBase
{
    [Property] public int Act { get; set; }
    [Property] public Func<bool> OnCheck { get; set; }

    public override void OnTouch(Player ply)
    {
        if (OnCheck is not null && OnCheck.Invoke() == false) return;

        LevelManager.Instance.CurrentLevel.Act(Act);
    }
}