using System.IO;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

// ReSharper disable RedundantDefaultMemberInitializer

#pragma warning disable CS0618

namespace JFUtils.WithPatch;

[PublicAPI]
public static class ImportantZDOs
{
    public static readonly int ZDO_Created_Hash = "JFUtils_ZDOcreated_Hash".GetStableHashCode();
    private static readonly ConditionalWeakTable<ZDOMan, ZDOManAdditionalData> data = new();
    public static List<ImportantZDO_Settings> settings = new();
    public static bool enabled = false;

    private static ZDOManAdditionalData GetAdditionalData(this ZDOMan zdoMan) { return data.GetOrCreateValue(zdoMan); }

    public static List<ZDO> GetAllImportantZDOs(this ZDOMan zdoMan)
    {
        var result = zdoMan.GetAdditionalData().importantZDOs;
        result.RemoveAll(x => x.Valid == false);
        return result;
    }

    public static HashSet<ZDO> GetImportantZDOs(this ZDOMan zdoMan, int prefabHash)
    {
        return zdoMan.GetAdditionalData()
            .importantZDOs.Where(zdo => zdo.GetPrefab() == prefabHash).ToHashSet();
    }

    public static void RegisterImportantZDO(ImportantZDO_Settings ZDO_settings)
    {
        if (!enabled) throw new UnityException("ImportantZDOs is disabled.");
        if (settings.Any(x => x.prefabHash == ZDO_settings.prefabHash))
            throw new ArgumentException($"{ZDO_settings.prefabHash} is already registered as important ZDO.");
        settings.Add(ZDO_settings);
    }

    public static void RegisterImportantZDO(this ZDOMan zdoMan, ImportantZDO_Settings ZDO_settings)
    {
        if (!enabled) throw new UnityException("ImportantZDOs is disabled.");
        RegisterImportantZDO(ZDO_settings);
    }


    [Obsolete("Use carefully! it should be internally called.")]
    public static void AddImportantZDO(this ZDOMan zdoMan, ZDO zdo, int prefab)
    {
        if (!enabled) throw new UnityException("ImportantZDOs is disabled.");
        if (prefab != 0 && settings.All(x => x.prefabHash != prefab))
            throw new UnityException(
                "Tried to add an important ZDO that isn't registered. Make sure to call RegisterImportantZDO() first.");

        zdoMan.GetAllImportantZDOs().Add(zdo);
        ImportantZDOs.settings.Find(x => x.prefabHash == prefab)?.onAdded?.Invoke();
    }

    [Obsolete("Use carefully! it should be internally called.")]
    public static void RemoveImportantZDO(this ZDOMan zdoMan, ZDO zdo)
    {
        if (!enabled) throw new UnityException("ImportantZDOs is disabled.");
        var importantZDOs = zdoMan.GetAllImportantZDOs();
        importantZDOs.Remove(zdo);
        ImportantZDOs.settings.Find(x => x.prefabHash == zdo.GetPrefab())?.onRemoved?.Invoke();
    }

    [Serializable]
    public class ZDOManAdditionalData
    {
        public List<ZDO> importantZDOs;

        public ZDOManAdditionalData() { importantZDOs = new List<ZDO>(); }
    }

    [HarmonyPatch]
    private static class Patch
    {
        [HarmonyPatch(typeof(ZDOMan), nameof(ZDOMan.HandleDestroyedZDO))]
        [HarmonyPrefix]
        private static void HandleDestroyedZDO(ZDOMan __instance, ZDOID uid)
        {
            if (!enabled) return;
            var zdo = __instance.GetZDO(uid);
            if (zdo == null) return;
            if (settings.Any(x => x.prefabHash == zdo.GetPrefab()))
                __instance.RemoveImportantZDO(zdo);
        }

        [HarmonyPatch(typeof(ZDOMan), nameof(ZDOMan.CreateNewZDO), typeof(ZDOID), typeof(Vector3), typeof(int))]
        [HarmonyPostfix]
        private static void CreateNewZDO(ZDOMan __instance, ZDOID uid, Vector3 position, int prefabHashIn)
        {
            if (!enabled) return;
            var zdo = __instance.m_objectsByID[uid];
            if (zdo == null) return;
            var zdoSettings = settings.Find(x => x.prefabHash == prefabHashIn);
            if (zdoSettings != null)
            {
                if (zdoSettings.trackCreationTime) zdo.Set(ZDO_Created_Hash, DateTimeOffset.UtcNow.Ticks);

                __instance.AddImportantZDO(zdo, prefabHashIn);
            }
        }

        [HarmonyPatch(typeof(ZDOMan), nameof(ZDOMan.Load), typeof(BinaryReader), typeof(int))]
        [HarmonyPostfix]
        private static void Load(ZDOMan __instance)
        {
            if (!enabled) return;
            foreach (var zdo in __instance.m_objectsByID.Values)
            {
                var prefab = zdo.GetPrefab();
                if (settings.Any(x => x.prefabHash == prefab))
                    __instance.AddImportantZDO(zdo, prefab);
            }
        }
    }
}

[PublicAPI]
public class ImportantZDO_Settings
{
    public ImportantZDO_Settings(int prefabHash, bool trackCreationTime = false,
        Action onRemoved = null, Action onAdded = null)
    {
        this.prefabHash = prefabHash;
        this.trackCreationTime = trackCreationTime;
        this.onRemoved = onRemoved;
        this.onAdded = onAdded;
    }

    public int prefabHash { get; }
    public bool trackCreationTime { get; }
    public Action onRemoved { get; }

    public Action onAdded { get; }

    public static implicit operator bool(ImportantZDO_Settings x) => x != null;
}