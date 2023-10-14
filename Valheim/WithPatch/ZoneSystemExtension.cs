namespace JFUtils.Valheim.WithPatch;

[HarmonyPatch]
public class ZoneSystemExtension
{
    [HarmonyPatch(typeof(ZoneSystem), nameof(ZoneSystem.RegisterLocation))]
    [HarmonyPrefix, HarmonyWrapSafe]
    private static bool Patch(ZoneLocation location, Vector3 pos)
    {
        if (!Valheim.ZoneSystemExtension.creatingValidPlacesForLocation) return true;
        if (Valheim.ZoneSystemExtension.creatingPlacesFor != location.m_prefabName) return true;
        Valheim.ZoneSystemExtension.tempPoints.Add(pos);
        return false;
    }
}