public class TriggerFinish : TriggerBase
{
    public override void OnTouch(Player ply)
    {
        if (!LevelManager.Instance.CurrentLevel.CheckFinish())
        {
            //todo sound error
            ply.HeaderLevel.Show();

            return;
        } 

        LevelManager.Instance.Finish();
    }
}