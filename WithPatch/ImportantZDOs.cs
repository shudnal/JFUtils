using System.IO;
using System.Runtime.CompilerServices;

namespace JFUtils.Valheim;

public static class ImportantZDOs
{
    public static readonly int ZDO_Created_Hash = "JFUtils_ZDOcreated_Hash".GetStableHashCode();
    private static readonly ConditionalWeakTable<ZDOMan, ZDOManAdditionalData> data = new();
    public static List<ImportantZDO_Settings> importantPrefabsHashes = new();
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

    public static void RegisterImportantZDO(int prefabHash, bool trackCreationTime)
    {
        if (!enabled) throw new UnityException("ImportantZDOs is disabled.");
        if (importantPrefabsHashes.Any(x => x.prefabHash == prefabHash))
            DebugWarning($"{prefabHash} is already registered as important ZDO.");
        importantPrefabsHashes.Add(new(prefabHash, trackCreationTime));
    }

    public static void RegisterImportantZDO(this ZDOMan zdoMan, int prefabHash, bool trackCreationTime = false)
    {
        if (!enabled) throw new UnityException("ImportantZDOs is disabled.");
        RegisterImportantZDO(prefabHash, trackCreationTime);
    }

    private static bool AddImportantZDO(this ZDOMan zdoMan, ZDO zdo)
    {
        if (!enabled) throw new UnityException("ImportantZDOs is disabled.");
        var prefab = zdo.GetPrefab();
        if (prefab != 0 && !importantPrefabsHashes.Any(x => x.prefabHash == prefab))
            throw new UnityException(
                "Tried to add an important ZDO that isn't registered. Make sure to call RegisterImportantZDO() first.");

        return zdoMan.GetAllImportantZDOs().Add(zdo);
    }

    private static bool RemoveImportantZDO(this ZDOMan zdoMan, ZDO zdo)
    {
        if (!enabled) throw new UnityException("ImportantZDOs is disabled.");
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
            if (importantPrefabsHashes.Any(x => x.prefabHash == zdo.GetPrefab()))
                __instance.RemoveImportantZDO(zdo);
        }

        [HarmonyPatch(typeof(ZDOMan), nameof(ZDOMan.CreateNewZDO), typeof(ZDOID), typeof(Vector3), typeof(int))]
        [HarmonyPostfix]
        private static void CreateNewZDO(ZDOMan __instance, ZDOID uid, Vector3 position, int prefabHashIn)
        {
            if (!enabled) return;
            var zdo = __instance.m_objectsByID[uid];
            if (zdo == null) return;
            var zdoSettings = importantPrefabsHashes.Find(x => x.prefabHash == prefabHashIn);
            if (zdoSettings != null)
            {
                if (zdoSettings.trackCreationTime) zdo.Set(ZDO_Created_Hash, DateTimeOffset.UtcNow.Ticks);

                __instance.AddImportantZDO(zdo);
            }
        }

        [HarmonyPatch(typeof(ZDOMan), nameof(ZDOMan.Load), typeof(BinaryReader), typeof(int))]
        [HarmonyPostfix]
        private static void Load(ZDOMan __instance)
        {
            if (!enabled) return;
            foreach (var zdo in __instance.m_objectsByID.Values)
                if (importantPrefabsHashes.Any(x => x.prefabHash == zdo.GetPrefab()))
                    __instance.AddImportantZDO(zdo);
        }
    }
}

public record ImportantZDO_Settings(int prefabHash, bool trackCreationTime)
{
    public int prefabHash { get; } = prefabHash;
    public bool trackCreationTime { get; } = trackCreationTime;
}