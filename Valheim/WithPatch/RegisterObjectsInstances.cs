using System.Reflection;
using Debug = UnityEngine.Debug;
using static System.Reflection.BindingFlags;

namespace Extensions.Valheim.WithPatch;

[HarmonyPatch]
internal static class RegisterObjectsInstances
{
    internal static List<Pickable> AllPickables { get; private set; } = new();
    internal static List<Plant> AllPlants { get; private set; } = new();
    internal static List<Door> AllDoors { get; private set; } = new();
    internal static List<Sign> AllSigns { get; private set; } = new();
    internal static List<Container> AllContainers { get; private set; } = new();
    internal static List<Bed> AllBeds { get; private set; } = new();

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Pickable), nameof(Pickable.Awake))]
    [HarmonyPatch(typeof(Plant), nameof(Plant.Awake))]
    [HarmonyPatch(typeof(Door), nameof(Door.Awake))]
    [HarmonyPatch(typeof(Sign), nameof(Sign.Awake))]
    [HarmonyPatch(typeof(Container), nameof(Container.Awake))]
    [HarmonyPatch(typeof(Bed), nameof(Bed.Awake))]
    private static void AddToInstanceCollection(MonoBehaviour __instance) =>
        __instance.StartCoroutine(enumerator(__instance));

    private static IEnumerator enumerator<T>(T component) where T : MonoBehaviour
    {
        var m_nview = (ZNetView)component.GetType().GetField("m_nview", NonPublic | Instance).GetValue(component);
        yield return new WaitWhile(() => m_nview.m_ghost || !m_nview.IsValid());

        if (component is Pickable pickable)
        {
            AllPickables.TryAdd(pickable);
            AllPickables.RemoveAll(item => item == null);
        } else if (component is Plant plant)
        {
            AllPlants.TryAdd(plant);
            AllPlants.RemoveAll(item => item == null);
        } else if (component is Door door)
        {
            AllDoors.TryAdd(door);
            AllDoors.RemoveAll(item => item == null);
        } else if (component is Sign sign)
        {
            AllSigns.TryAdd(sign);
            AllSigns.RemoveAll(item => item == null);
        } else if (component is Container container)
        {
            AllContainers.TryAdd(container);
            AllContainers = AllContainers.Where(x => x != null).ToList();
        } else if (component is Bed bed)
        {
            AllBeds.TryAdd(bed);
            AllBeds.RemoveAll(item => item == null);
        }
    }
}

public static class ObjectsInstances
{
    static ObjectsInstances()
    {
        new Harmony("Extensions.Valheim.WithPatch.RegisterObjectsInstances").PatchAll(typeof(RegisterObjectsInstances));
        Debug.Log("RegisterObjectsInstances patched");
    }

    public static List<Pickable> allPickables => RegisterObjectsInstances.AllPickables;
    public static List<Plant> allPlants => RegisterObjectsInstances.AllPlants;
    public static List<Door> allDoors => RegisterObjectsInstances.AllDoors;
    public static List<Sign> allSigns => RegisterObjectsInstances.AllSigns;
    public static List<Container> allContainers => RegisterObjectsInstances.AllContainers;
    public static List<CraftingStation> allCraftingStations => CraftingStation.m_allStations;
    public static List<Bed> allBeds => RegisterObjectsInstances.AllBeds;
}