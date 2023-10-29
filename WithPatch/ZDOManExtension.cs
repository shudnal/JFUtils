using System.IO;
using System.Runtime.CompilerServices;

namespace JFUtils.Valheim;

public static class ZDOManExtension
{
    private static readonly ConditionalWeakTable<ZDOMan, ZDOManAdditionalData> data = new();
    public static HashSet<int> importantPrefabsHashes = new();
    public static bool enabled = false;

    private static ZDOManAdditionalData GetAdditionalData(this ZDOMan zdoMan) { return data.GetOrCreateValue(zdoMan); }

    public static HashSet<ZDO> GetAllImportantZDOs(this ZDOMan zdoMan)
    {
        return zdoMan.GetAdditionalData()
            .importantZDOs;
    }

    public static HashSet<ZDO> GetImportantZDOs(this ZDOMan zdoMan, int prefabHash)
    {
        return zdoMan.GetAdditionalData()
            .importantZDOs.Where(zdo => zdo.GetPrefab() == prefabHash).ToHashSet();
    }

    public static void RegisterImportantZDO(int prefabHash)
    {
        if (!enabled) throw new UnityException("ZDOManExtension is disabled.");
        if (importantPrefabsHashes.Contains(prefabHash))
            DebugWarning($"{prefabHash} is already registered as important ZDO.");
        importantPrefabsHashes.Add(prefabHash);
    }

    public static void RegisterImportantZDO(this ZDOMan zdoMan, int prefabHash)
    {
        if (!enabled) throw new UnityException("ZDOManExtension is disabled.");
        RegisterImportantZDO(prefabHash);
    }

    private static bool AddImportantZDO(this ZDOMan zdoMan, ZDO zdo)
    {
        if (!enabled) throw new UnityException("ZDOManExtension is disabled.");
        var prefab = zdo.GetPrefab();
        if (prefab != 0 && !importantPrefabsHashes.Contains(prefab))
            throw new UnityException(
                "Tried to add an important ZDO that isn't registered. Make sure to call RegisterImportantZDO() first.");

        return zdoMan.GetAllImportantZDOs().Add(zdo);
    }

    private static bool RemoveImportantZDO(this ZDOMan zdoMan, ZDO zdo)
    {
        if (!enabled) throw new UnityException("ZDOManExtension is disabled.");
        return zdoMan.GetAllImportantZDOs().Remove(zdo);
    }

    [Serializable]
    public class ZDOManAdditionalData
    {
        public HashSet<ZDO> importantZDOs;

        public ZDOManAdditionalData() { importantZDOs = new HashSet<ZDO>(); }
    }

    [HarmonyPatch]
    private static class Patch
    {
        [HarmonyPatch(typeof(ZDOMan), nameof(ZDOMan.HandleDestroyedZDO))]
        [HarmonyPostfix]
        private static void HandleDestroyedZDO(ZDOMan __instance, ZDOID uid)
        {
            if (!enabled) return;
            var zdo = __instance.GetZDO(uid);
            if (zdo == null) return;
            if (importantPrefabsHashes.Contains(zdo.GetPrefab()))
                __instance.RemoveImportantZDO(zdo);
        }

        [HarmonyPatch(typeof(ZDOMan), nameof(ZDOMan.CreateNewZDO), typeof(ZDOID), typeof(Vector3), typeof(int))]
        [HarmonyPostfix]
        private static void CreateNewZDO(ZDOMan __instance, ZDOID uid, Vector3 position, int prefabHashIn)
        {
            if (!enabled) return;
            var zdo = __instance.m_objectsByID[uid];
            if (zdo == null) return;
            if (importantPrefabsHashes.Contains(prefabHashIn)) __instance.AddImportantZDO(zdo);
        }

        [HarmonyPatch(typeof(ZDOMan), nameof(ZDOMan.Load), typeof(BinaryReader), typeof(int))]
        [HarmonyPostfix]
        private static void Load(ZDOMan __instance)
        {
            if (!enabled) return;
            foreach (var zdo in __instance.m_objectsByID.Values)
                if (importantPrefabsHashes.Contains(zdo.GetPrefab()))
                    __instance.AddImportantZDO(zdo);
        }
    }
}