namespace JFUtils.Valheim.WithPatch;

[HarmonyPatch]
public class UpdateConfigOnGameStart
{
    [HarmonyPatch(typeof(Game), nameof(Game.Start))]
    [HarmonyPostfix]
    private static void UpdateConfig() { plugin?.Config.Reload(); }
}