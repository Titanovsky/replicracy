public static class GlobalSettings
{
    [ConVar("rep_debug", ConVarFlags.GameSetting)] public static bool IsDebug { get; set; } = false;
}