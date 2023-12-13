using static JFUtils.WithPatch.ObjectsInstances;

namespace JFUtils.WithPatch;

[HarmonyPatch]
internal static class RegisterObjectsInstances
{
    internal static List<Pickable> AllPickables { get; } = new();
    internal static List<Plant> AllPlants { get; } = new();
    internal static List<Door> AllDoors { get; } = new();
    internal static List<Sign> AllSigns { get; } = new();
    internal static List<Container> AllContainers { get; private set; } = new();
    internal static List<Bed> AllBeds { get; } = new();

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Pickable), nameof(Pickable.Awake))]
    [HarmonyPatch(typeof(Plant), nameof(Plant.Awake))]
    [HarmonyPatch(typeof(Door), nameof(Door.Awake))]
    [HarmonyPatch(typeof(Sign), nameof(Sign.Awake))]
    [HarmonyPatch(typeof(Container), nameof(Container.Awake))]
    [HarmonyPatch(typeof(Bed), nameof(Bed.Awake))]
    private static void AddToInstanceCollection(MonoBehaviour __instance)
    {
        if (enabled == false) return;
        __instance.StartCoroutine(enumerator(__instance));
    }

    private static IEnumerator enumerator<T>(T component) where T : MonoBehaviour
    {
        var m_nview = component.GetZNetView();
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

[PublicAPI]
public static class ObjectsInstances
{
    internal static bool enabled;

    static ObjectsInstances()
    {
        new Harmony("JFUtils.Valheim.RegisterObjectsInstances").PatchAll(typeof(RegisterObjectsInstances));
    }

    public static List<Pickable> AllPickables => RegisterObjectsInstances.AllPickables;
    public static List<Plant> AllPlants => RegisterObjectsInstances.AllPlants;
    public static List<Door> AllDoors => RegisterObjectsInstances.AllDoors;
    public static List<Sign> AllSigns => RegisterObjectsInstances.AllSigns;
    public static List<Container> AllContainers => RegisterObjectsInstances.AllContainers;
    public static List<CraftingStation> AllCraftingStations => CraftingStation.m_allStations;
    public static List<Bed> AllBeds => RegisterObjectsInstances.AllBeds;
}