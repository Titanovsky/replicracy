public class LevelArea : LevelBase
{
    [Property, Group("Base")] public override string Class { get; set; } = "level3";
    [Property, Group("Base")] public override string Name { get; set; } = "Area 69";
    [Property, Group("Base")] public override SceneFile NextLevelScene { get; set; }
    [Property, Group("Base")] public override SceneFile CurrentLevelScene { get; set; }
    [Property, Group("Base")] public override float MinDangerousZ { get; set; } = -310;

    [Property, Group("Stats")] public override int NextDna { get; set; } = 0;
    [Property, Group("Stats")] public override int NextFrags { get; set; } = 5;

    [Property, Feature("Area 69")] public string Fuck { get; set; } = "das";

    public override void Act(int act)
    {
        switch (act)
        {
            default:
                break;
            case 1:
                Act1(); 
                break;
            case 2:
                Act2();
                break;
            case 3:
                Act3();
                break;
            case 4:
                Act4();
                break;
        }
    }

    private void Act1()
    { }

    private void Act2()
    { }

    private void Act3()
    { }

    private void Act4()
    { }
}