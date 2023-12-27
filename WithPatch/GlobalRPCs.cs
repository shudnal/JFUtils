namespace JFUtils.WithPatch;

[HarmonyPatch]
public static class GlobalRPCs
{
    [HarmonyPatch(typeof(Game), nameof(Game.Start)), HarmonyPostfix]
    private static void Patch()
    {
        if (ZRoutedRpc.instance.m_functions.ContainsKey("JFUtils_MoveZDO".GetStableHashCode())) return;
        ZRoutedRpc.instance.Register<ZDOID, Vector3>("JFUtils_MoveZDO", MoveZDO);
    }

    private static void MoveZDO(long _, ZDOID zdoid, Vector3 position)
    {
        if (!ZDOMan.instance.m_objectsByID.TryGetValue(zdoid, out var zdo)) return;
        zdo.SetPosition(position);
    }
}