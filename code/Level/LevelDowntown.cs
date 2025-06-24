public class LevelDowntown : LevelBase
{
    [Property] public override string Class { get; set; } = "level1";
    [Property] public override string Name { get; set; } = "Downtown";
    [Property] public override SceneFile NextLevelScene { get; set; }

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