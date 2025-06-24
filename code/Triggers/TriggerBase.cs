public abstract class TriggerBase : Component, Component.ITriggerListener
{
    public virtual void OnTouch(Player ply)
    {

    }
    
    public void OnTriggerEnter(Collider other)
    {
        var ply = other.Components.GetInAncestorsOrSelf<Player>();
        if (!ply.IsValid()) return;

        OnTouch(ply);
    }
}