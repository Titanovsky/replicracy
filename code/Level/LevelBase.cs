public abstract class LevelBase : Component
{
    [Property, Group("Base")] public virtual string Class { get; set; } = "1";
    [Property, Group("Base")] public virtual string Name { get; set; } = "None";
    [Property, Group("Base")] public virtual SceneFile NextLevelScene { get; set; }

    [Property, Group("Stats")] public virtual int NextDna { get; set; } = 0;
    [Property, Group("Stats")] public virtual int NextFrags { get; set; } = 0;

    public virtual void Act(int act) { }

    public virtual bool CheckFinish() { return false; }
}