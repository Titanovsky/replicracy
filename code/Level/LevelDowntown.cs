public sealed class LevelDowntown : LevelBase
{
    [Property, Group("Base")] public override string Class { get; set; } = "level1";
    [Property, Group("Base")] public override string Name { get; set; } = "Downtown";
    [Property, Group("Base")] public override SceneFile NextLevelScene { get; set; }

    [Property, Group("Stats")] public override int NextDna { get; set; } = 15;
    [Property, Group("Stats")] public override int NextFrags { get; set; } = 25;

    [Property, Feature("Downtown")] public string Fuck { get; set; } = "das";

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
    {
        Log.Info($"The first act of start!");
    }

    private void Act2()
    { 
        //todo
    }

    private void Act3()
    {
        //todo
    }

    private void Act4()
    {
        //todo
    }
}