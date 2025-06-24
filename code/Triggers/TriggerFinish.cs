public class TriggerFinish : Component, Component.ITriggerListener
{
    public void OnTriggerEnter(Collider other)
    {
        var ply = other.Components.GetInAncestorsOrSelf<Player>();
        if (!ply.IsValid()) return;

        if (!LevelManager.Instance.CurrentLevel.CheckFinish())
        {
            //todo sound error
            ply.HeaderLevel.Show();

            return;
        } 

        LevelManager.Instance.Finish();
    }
}