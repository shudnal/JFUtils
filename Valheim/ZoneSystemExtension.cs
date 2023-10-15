using System.Threading.Tasks;

namespace JFUtils.Valheim;

public static class ZoneSystemExtension
{
    internal static List<Vector3> tempPoints = new();
    internal static bool creatingValidPlacesForLocation;
    internal static string creatingPlacesFor = "";

    public static void SetGlobalKey(this ZoneSystem zoneSystem, string key, object value) =>
        zoneSystem.SetGlobalKey($"{key} {value}");

    public static string GetGlobalKeyValue(this ZoneSystem zoneSystem, string key)
    {
        zoneSystem.GetGlobalKey(key, out var value);
        return value;
    }

    public static string GetOrAddGlobalKey(this ZoneSystem zoneSystem, string key, string defaultValue)
    {
        if (zoneSystem.GetGlobalKey(key, out var value)) return value;

        zoneSystem.SetGlobalKey(key, defaultValue);
        return defaultValue;
    }

    public static (Vector2i, LocationInstance)[] GetGeneratedLocationsByName(this ZoneSystem zoneSystem,
        string key) =>
        instance.m_locationInstances
            .Where(x => x.Value.m_location.m_prefabName == key)
            .Select(x => (x.Key, x.Value))
            .ToArray();

    public static List<Vector3> CreateValidPlacesForLocation(this ZoneSystem zoneSystem, string key, int count)
    {
        tempPoints.Clear();
        var location = instance.GetLocation(key);
        if (location == null)
        {
            DebugWarning($"Could not find location with name '{key}'");
            return null;
        }

        creatingPlacesFor = location.m_prefabName;
        creatingValidPlacesForLocation = true;
        var haldorLocationsVanillaCount = location.m_quantity;
        location.m_quantity = count;
        var isUnique = location.m_unique;
        location.m_unique = false;

        instance.GenerateLocations(location);
        location.m_quantity = haldorLocationsVanillaCount;
        location.m_unique = isUnique;
        creatingPlacesFor = string.Empty;
        creatingValidPlacesForLocation = false;

        return tempPoints;
    }

    public static Task<List<ZDO>> GetWorldObjectsAsync(this ZoneSystem zoneSystem,
        params Func<ZDO, bool>[] customFilters) =>
        Task.Run(() =>
        {
            var zdos = new List<ZDO>(ZDOMan.instance.m_objectsByID.Values);
            if (customFilters != null && customFilters.Length != 0)
                zdos = zdos.Where(x => customFilters.All(filter => filter?.Invoke(x) ?? false)).ToList();
            return zdos;
        });

    public static async Task<List<ZDO>> GetWorldObjectsAsync(this ZoneSystem zoneSystem, string prefabName,
        params Func<ZDO, bool>[] customFilters)
    {
        int prefabHash = prefabName.GetStableHashCode();
        Func<ZDO, bool> prefabFilter = zdo => zdo.GetPrefab() == prefabHash;
        return await zoneSystem.GetWorldObjectsAsync(customFilters.AddToArray(prefabFilter));
    }
}