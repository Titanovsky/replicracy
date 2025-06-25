public abstract class ReplicantState
{
    protected Replicant Replicant { get; private set; }

    public ReplicantState(Replicant replicant)
    {
        Replicant = replicant;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}