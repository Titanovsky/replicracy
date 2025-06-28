public class TriggerFinish : TriggerBase
{
    public override void OnTouch(Player ply)
    {
        if (!LevelManager.Instance.CurrentLevel.CheckFinish())
        {
            ply.Error();
            ply.HeaderLevel.Show();

            return;
        } 

        LevelManager.Instance.Finish();
    }
}