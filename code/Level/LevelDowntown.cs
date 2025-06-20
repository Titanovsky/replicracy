public class LevelDowntown : LevelBase
{
    public override string Class { get; set; } = "level1";
    public override string Name { get; set; } = "Downtown";
    public override LevelBase NextLevel { get; set; }

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