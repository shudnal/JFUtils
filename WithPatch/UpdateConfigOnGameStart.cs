using BepInEx;

namespace JFUtils.Valheim.WithPatch;

[HarmonyPatch]
public class UpdateConfigOnGameStart
{
    [HarmonyPatch(typeof(Game), nameof(Game.Start))]
    [HarmonyPostfix, HarmonyWrapSafe]
    private static void UpdateConfig() => GetPlugin<BaseUnityPlugin>()?.Config.Reload();
}