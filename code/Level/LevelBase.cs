public abstract class LevelBase : Component
{
    public virtual string Class { get; set; } = "1";
    public virtual string Name { get; set; } = "None";
    public virtual LevelBase NextLevel { get; set; }

    public virtual void Act(int act) { }
}