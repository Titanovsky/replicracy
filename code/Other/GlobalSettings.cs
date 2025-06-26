public static class GlobalSettings
{
    [ConVar("rep_debug", ConVarFlags.GameSetting)] public static bool IsDebug { get; set; } = false;

    [ConVar("rep_max_replicants", ConVarFlags.GameSetting)] public static int MaxReplicants { get; set; } = 8;

    public static int CostReplicate { get; set; } = 5;
    public static int CostHeal { get; set; } = 2;
    public static int CostBodyHead { get; set; } = 5;
    public static int CostBodyLeftHand { get; set; } = 1;
    public static int CostBodyRightHand { get; set; } = 1;
    public static int CostBodyLeftLeg { get; set; } = 2;
    public static int CostBodyRightLeg { get; set; } = 2;
    public static List<int> CostAbility { get; set; } = new()
    {
        5,
        10,
        15,
        20,
        25,
        30
    };
}