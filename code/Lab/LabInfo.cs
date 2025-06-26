public sealed class LabInfo : Component
{
    public static LabInfo Instance {  get; private set; }

    [Property, Feature("Body")] public int BodyCost { get; set; } = 0;
    [Property, Feature("Body"), Range(0, 3)] public int BodyCurrentHead { get; set; } = 0;
    [Property, Feature("Body"), Range(0, 3)] public int BodyCurrentLeftHand { get; set; } = 0;
    [Property, Feature("Body"), Range(0, 3)] public int BodyCurrentRightHand { get; set; } = 0;
    [Property, Feature("Body"), Range(0, 3)] public int BodyCurrentLeftLeg { get; set; } = 0;
    [Property, Feature("Body"), Range(0, 3)] public int BodyCurrentRightLeg { get; set; } = 0;
    [Property, Feature("Body"), Range(0, 3)] public int BodyLabHead { get; set; } = 0;
    [Property, Feature("Body"), Range(0, 3)] public int BodyLabLeftHand { get; set; } = 0;
    [Property, Feature("Body"), Range(0, 3)] public int BodyLabRightHand { get; set; } = 0;
    [Property, Feature("Body"), Range(0, 3)] public int BodyLabLeftLeg { get; set; } = 0;
    [Property, Feature("Body"), Range(0, 3)] public int BodyLabRightLeg { get; set; } = 0;

    [Property, Feature("Abilities")]
    public List<bool> UnlockedAbitilies { get; set; } = new()
    {
        false,
        false,
        false,
        false,
        false,
        false
    };

    private void CreateSingleton()
    {
        if (Instance is null)
            Instance = this;
    }

    private void DestroySingleton()
    {
        Instance = null; //? maybe should be remove cuz Save
    }

    protected override void OnAwake()
    {
        CreateSingleton();
    }

    protected override void OnDestroy()
    {
        DestroySingleton();
    }
}