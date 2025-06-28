using Sandbox.Diagnostics;

public abstract class LevelBase : Component
{
    [Property, Group("Base")] public virtual string Class { get; set; } = "1";
    [Property, Group("Base")] public virtual string Name { get; set; } = "None";
    [Property, Group("Base")] public virtual SceneFile NextLevelScene { get; set; }
    [Property, Group("Base")] public virtual SceneFile CurrentLevelScene { get; set; }
    [Property, Group("Base")] public virtual SoundEvent Music { get; set; }
    [Property, Group("Base")] public virtual float MinDangerousZ { get; set; } = -500;

    [Property, Group("Stats")] public virtual int NextDna { get; set; } = 0;
    [Property, Group("Stats")] public virtual int NextFrags { get; set; } = 0;


    public virtual void Act(int act) { }

    public virtual bool CheckFinish() 
    {
        if (Player.Instance.CollectDna < NextDna) return false;
        if (Player.Instance.Frags < NextFrags) return false;

        return true;
    }
}