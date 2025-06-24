public abstract class LevelBase : Component
{
    [Property] public virtual string Class { get; set; } = "1";
    [Property] public virtual string Name { get; set; } = "None";
    [Property] public virtual SceneFile NextLevelScene { get; set; }

    public virtual void Act(int act) { }

    public virtual bool CheckFinish() { return false; }
}