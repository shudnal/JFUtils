namespace JFUtils.WithPatch;

[HarmonyPatch]
public class ZoneSystemExtension_Patch
{
    [HarmonyPatch(typeof(ZoneSystem), nameof(ZoneSystem.RegisterLocation))]
    [HarmonyPrefix, HarmonyWrapSafe]
    private static bool Patch(ZoneLocation location, Vector3 pos)
    {
        if (!JFUtils.ZoneSystemExtension.creatingValidPlacesForLocation) return true;
        if (JFUtils.ZoneSystemExtension.creatingPlacesFor != location.m_prefabName) return true;
        JFUtils.ZoneSystemExtension.tempPoints.Add(pos);
        return false;
    }
}