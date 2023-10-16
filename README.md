# JF Utils

Adds some usefull utils for programmers.<br>
Includes ServerSync.dll inside.<br>
If you have any questions or suggestions please message me in discord: ```justafrogger```<br>

### Merging the DLLs into your mod

Download the JFUtils.dll and the ServerSync.dll from the release section to the right.
Including the DLLs is best done via ILRepack (https://github.com/ravibpatel/ILRepack.Lib.MSBuild.Task). You can load
this package (ILRepack.Lib.MSBuild.Task) from NuGet.

If you have installed ILRepack via NuGet, simply create a file named `ILRepack.targets` in your project and copy the
following content into the file

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <Target Name="ILRepacker" AfterTargets="Build">
        <ItemGroup>
            <InputAssemblies Include="$(TargetPath)"/>
            <InputAssemblies Include="$(OutputPath)\JFUtils.dll"/>
        </ItemGroup>
        <ILRepack Parallel="true" DebugInfo="true" Internalize="true" InputAssemblies="@(InputAssemblies)"
                  OutputFile="$(TargetPath)" TargetKind="SameAsPrimaryAssembly" LibraryPath="$(OutputPath)"/>
    </Target>
</Project>
```

Make sure to set the JFUtils.dll in your project to "Copy to output directory" in the properties of the DLLs and to add
a reference to it.

## Documentation
- Classes
- - [ModBase](#ModBase)
- - - [Methods](#ModBase-methods)
- - - [Properties](#ModBase-properties)
- - - [Events](#ModBase-events)
- - - [Fields](#ModBase-fields)
- - [TimeUtils](#TimeUtils)
- - - [Methods](#TimeUtils-methods)
- - [SimpleVector2](#SimpleVector2)
- - - [Methods](#SimpleVector2-methods)
- - - [Fields](#SimpleVector2-fields)
- - - [ExtensionMethods](#SimpleVector2-extensions)
- - [SimpleVector3](#SimpleVector3)
- - - [Methods](#SimpleVector3-methods)
- - - [Fields](#SimpleVector3-fields)
- - - [ExtensionMethods](#SimpleVector3-extensions)
- - [ObjectsInstances](#ObjectsInstances)
- - - [Properties](#ObjectsInstances-properties)
- - [ConsoleCommandException](#ConsoleCommandException)
- - ConfigurationManagerAttributes
- Enums
- - [Wheather](#Wheather)
- Extensions
- - [ZoneSystemExtension](#ZoneSystemExtension)
- - [BiomeExtension](#BiomeExtension)
- - [ObjectDBExtension](#ObjectDBExtension)
- - [ItemDropExtension](#ItemDropExtension)
- - [MinimapExtension](#MinimapExtension)
- - [PrivateAreaExtension](#PrivateAreaExtension)
- - [RecipeExtension](#RecipeExtension)
- - [SkillsExtension](#SkillsExtension)
- - [StringExtension](#StringExtension)
- - [ZNetSceneExtension](#ZNetSceneExtension)
- - [VectorExtension](#VectorExtension)
- - [EnumerableExtension](#EnumerableExtension)
- - [GameObjectExtension](#GameObjectExtension)
- - [MonoBehaviourExtension](#MonoBehaviourExtension)
- - [RectExtension](#RectExtension)
- - [RendererExtension](#RendererExtension)
- - [TransformExtension](#TransformExtension)


## ModBase <a name="ModBase"></a>
All methods, fields and properties are static.
### Methods <a name="ModBase-methods"></a>
#### CreateMod
```void CreateMod(BaseUnityPlugin _plugin, string modName, string modAuthor, string modVersion, bool pathAll = true)```<br>
Allows you to create your own mod automatically.
Call ```ModBase.CreateMod()``` to create a mod. It will automatically:
- create ConfigSync
- SetupWatcher on config file
- call Harmony.PatchAll() - can be disabled via ```pathAll: false```
- Add config server lock<br>
```csharp
 private void Awake()
    {
        ModBase.CreateMod(_plugin: this, ModName, ModAuthor, ModVersion, pathAll: true);
        OnConfigurationChanged += () => {  };
    }
```
#### RunCommand
```void RunCommand(Terminal.ConsoleEvent action, Terminal.ConsoleEventArgs args)```<br>
When you adding a new command, you can call this method to format it. It automatically uses try catch.
throw new ConsoleCommandException to make error be shown only in in-game console. Any other exception will be shown in the log.
Example:<br>
```csharp

    [HarmonyPatch(typeof(Terminal), nameof(InitTerminal))]
    [HarmonyWrapSafe]
    internal class TerminalCommands
    {
        private static void Postfix()
        {
            new ConsoleCommand("mycoolcommand", "", args => RunCommand(args =>
            {
                if (!IsAdmin) throw new ConsoleCommandException("You are not an admin on this server.");
                //Do something
            }, args));
        }
    }
```
#### GetPlugin
```T GetPlugin<T>() where T : BaseUnityPlugin```<br>
```BaseUnityPlugin GetPlugin()```<br>
Returns the plugin instance of type of BaseUnityPlugin or of type of T. T should your BaseUnityPlugin type.<br>
```csharp
GetPlugin<MyMod>().SomeMethodImplodedInMyMod();
```
#### Debug
```void Debug(object msg, bool showInHud = false, bool showInConsole = false)```<br>
Logs message to the BepInEx console.
#### DebugError
```void DebugError(object msg, bool showWriteToDev = true, bool showInConsole = false)```<br>
Logs error message to the BepInEx console.
#### DebugWarning
```void DebugWarning(object msg, bool showWriteToDev = true, bool showInConsole = false)```<br>
Logs warning message to the BepInEx console.
#### DebugWarning
```AssetBundle LoadAssetBundle(string filename)```<br>
Loads an asset bundle, returns the it and saves it in ```ModBase.bundle``` field.
#### CreateModGUID
```string CreateModGUID(string ModName, string ModAuthor) => $"com.{ModAuthor}.{ModName}";```<br>
Constructs a mod GUID. in format ```com.ModAuthor.ModName```.
### Properties <a name="ModBase-properties"></a>
#### ModName
```string ModName { get; private set; }```<br>
The name of the mod.
#### ModAuthor
```string ModAuthor { get; private set; }```<br>
The author of the mod.
#### ModVersion
```string ModVersion { get; private set; }```<br>
The version of the mod.
#### ModGUID
```string ModGUID { get; private set; }```<br>
The GUID of the mod.
#### Harmony
```Harmony harmony { get; private set; }```<br>
The harmony instance for this mod.
#### IsAdmin
```bool IsAdmin```<br>
Whether the player is an admin on this server.
### Fields <a name="ModBase-fields"></a>
#### bundle
```AssetBundle bundle```<br>
The asset bundle of the mod set by ```LoadAssetBundle```.
### Events <a name="ModBase-events"></a>
#### OnConfigurationChanged
```Action OnConfigurationChanged```<br>
Called when configuration file is changed.
<br><br><br>
## TimeUtils <a name="TimeUtils"></a>
Made by [Azumatt](https://github.com/AzumattDev).<br>
All methods are static.
### Methods <a name="TimeUtils-methods"></a>
#### GetCurrentTimeValue
```(int, int) GetCurrentTimeValue()```<br>
Returns the current time in format ```HH.mm```.
<br><br><br>
## SimpleVector2 <a name="SimpleVector2"></a>
Simple struct for 2D vector. 
### Methods <a name="SimpleVector2-methods"></a>
#### ToSimpleVector2
```UnityEngine.Vector2 ToVector2()```<br>
Converts to UnityEngine.Vector2.
#### ToString
```string ToString()```<br>
Converts to string in format ```X: {X}, Y: {Y}```
### Fields <a name="SimpleVector2-fields"></a>
#### x
```float x```<br>
X value.
#### y
```float y```<br>
Y value.
### ExtensionMethods <a name="SimpleVector2-extensions"></a>
#### ToSimpleVector2
```SimpleVector2 ToSimpleVector2(this UnityEngine.Vector2 vector2)```<br>
Converts UnityEngine.Vector2 to SimpleVector2.
<br><br><br>
## SimpleVector3 <a name="SimpleVector3"></a>
Simple struct for 3D vector. 
### Methods <a name="SimpleVector3-methods"></a>
#### ToSimpleVector3
```UnityEngine.Vector3 ToVector3()```<br>
Converts to UnityEngine.Vector3.
#### ToString
```string ToString()```<br>
Converts to string in format ```X: {X}, Y: {Y}, Z: {Z}```
### Fields <a name="SimpleVector3-fields"></a>
#### x
```float x```<br>
X value.
#### y
```float y```<br>
Y value.
#### z
```float z```<br>
Z value.
### ExtensionMethods <a name="SimpleVector3-extensions"></a>
#### ToSimpleVector3
```SimpleVector3 ToSimpleVector3(this UnityEngine.Vector3 vector3)```<br>
Converts UnityEngine.Vector3 to SimpleVector3.
<br><br><br>
## ObjectsInstances <a name="ObjectsInstances"></a>
Registers all activly loaded pickables, plants, doors, signs, containers, crafting stations and beds.
### Properties <a name="ObjectsInstances-properties"></a>
#### AllPickables
```List<Pickable> AllPickables```<br>
List of all pickables.
#### AllPlants
```List<Plant> AllPlants```<br>
List of all plants.
#### AllDoors
```List<Door> AllDoors```<br>
List of all doors.
#### AllSigns
```List<Sign> AllSigns```<br>
List of all signs.
#### AllContainers
```List<Container> AllContainers```<br>
List of all containers.
#### AllCraftingStations
```List<CraftingStation> AllCraftingStations```<br>
List of all crafting stations.
#### AllBeds
```List<Bed> AllBeds```<br>
List of all beds.
<br><br><br>
## ConsoleCommandException <a name="ConsoleCommandException"></a>
If running console command using ModBase.RunCommand, you should throw this exception if some commands conditions are 
not met. It will be filtered from other exceptions and printed only in in-game console.
<br><br><br>
## Wheather <a name="Wheather"></a>
Enum of all posible vanila weather.
<br><br><br>
## ZoneSystemExtension <a name="ZoneSystemExtension"></a>
#### SetGlobalKey
```void SetGlobalKey(string key, object value)```<br>
Sets the global key by string key.
#### GetGlobalKeyValue
``` string GetGlobalKeyValue(string key)```<br>
Gets the global key value by string key.
#### GetOrAddGlobalKey
```string GetOrAddGlobalKey(string key, string defaultValue)```<br>
Gets or adds the global key by string key.
#### GetGeneratedLocationsByName
```(Vector2i, LocationInstance)[] GetGeneratedLocationsByName(string key)```<br>
Gets all generated locations by name. Returns an array of turple with Vector2i and LocationInstance, the first one is the 
zoneId, the second one is the location data.
#### CreateValidPlacesForLocation
```List<Vector3> CreateValidPlacesForLocation(string key, int count)```<br>
Returns valid places for location generated by vanilla algorithm.
#### GetWorldObjectsAsync
```Task<List<ZDO>> GetWorldObjectsAsync(params Func<ZDO, bool>[] customFilters)```<br>
```Task<List<ZDO>> GetWorldObjectsAsync(string prefabName, params Func<ZDO, bool>[] customFilters)```<br>
Returns ZDOs of all objects in world matching given filters. Should be awated.
<br><br><br>
## BiomeExtension <a name="BiomeExtension"></a>
#### GetLocalizationKey
```string GetLocalizationKey()```<br>
Returns the localization key for given biome.
<br><br><br>
## ObjectDBExtension <a name="ObjectDBExtension"></a>
#### GetItem
```ItemDrop GetItem(string name)```<br>
```ItemDrop GetItem(int hash)```<br>
Returns item by name or hash.
<br><br><br>
## ItemDropExtension <a name="ItemDropExtension"></a>
#### LocalizeName
```string ItemDrop.LocalizeName()```<br>
```string ItemDrop.ItemData.LocalizeName()```<br>
```string ItemDrop.ItemData.SharedData.LocalizeName()```<br>
Returns localized name of m_itemData.m_shared.m_name.
<br><br><br>
## MinimapExtension <a name="MinimapExtension"></a>
#### ForceUpdateLocationPins
```void ForceUpdateLocationPins()```<br>
Updates all location pins immediately.
<br><br><br>
## PrivateAreaExtension <a name="PrivateAreaExtension"></a>
#### InsideActiveFactionArea
```bool InsideActiveFactionArea(Vector3 point, Character.Faction faction)```<br>
Returns true if point is inside PrivateArea that is active and belongs to given faction.
<br><br><br>
## RecipeExtension <a name="RecipeExtension"></a>
#### ToList
```List<(ItemDrop.ItemData.SharedData, int)> ToList()```<br>
Converts Recipe to list of (ItemDrop.ItemData.SharedData, int). First is item shared data, second is amount.
<br><br><br>
## SkillsExtension <a name="SkillsExtension"></a>
#### GetCustomSkill
```Skill GetCustomSkill(string skillName)```<br>
Returns vanila or mod skill by given name.
<br><br><br>
## StringExtension <a name="StringExtension"></a>
#### Localize
```string Localize(string key)```<br>
Returns localized string.
#### IsGood
```bool IsGood(string str)```<br>
Returns true if string is not null and not empty.
<br><br><br>
## ZNetSceneExtension <a name="ZNetSceneExtension"></a>
#### GetItem
```ItemDrop GetItem(string name)```<br>
```ItemDrop GetItem(int hash)```<br>
Returns item by name or hash.
#### GetCharacter
```Character GetCharacter(string name)```<br>
```Character GetCharacter(int hash)```<br>
Returns character by name or hash.
<br><br><br>
## VectorExtension <a name="VectorExtension"></a>
#### ToV2 
```Vector2 ToV2()```<br>
Converts Vector3 to Vector2, but Vector2.y is set to Vector3.z.
#### ToV3
```Vector3 ToV3()```<br>
Converts Vector2 to Vector3, but Vector3.z is set to Vector2.y and Vector3.x is set to 0.
#### RoundCords
```Vector3 RoundCords()```<br>
```Vector2 RoundCords()```<br>
Returns Vector3 or Vector2 with coordinates casted to int.
#### SetX, SetY, SetZ
```ref Vector3 SetX(float x)```<br>
```ref Vector3 SetY(float y)```<br>
```ref Vector3 SetZ(float z)```<br>
```ref Vector2 SetX(float x)```<br>
```ref Vector2 SetY(float y)```<br>
Sets coordinates of Vector3 to given values.
#### DistanceXZ
```float DistanceXZ(Vector3 pos, Vector3 otherPos)```<br>
```float DistanceXZ(Vector3 pos, Transform otherPos)```<br>
```float DistanceXZ(Vector3 pos, Component otherPos)```<br>
```float DistanceXZ(Vector3 pos, GameObject otherPos)```<br>
Equals to Utils.DistanceXZ(Vector3 v0, Vector3 v1).
<br><br><br>
## EnumerableExtension <a name="EnumerableExtension"></a>
#### MakeDictionary
```Dictionary<TKey, TValue> MakeDictionary<TKey, TValue>```<br>
Converts IEnumerable<KeyValuePair<TKey, TValue>> to Dictionary<TKey, TValue>.
#### Random
```T Random()```<br>
Returns random element from IList<T>.
#### RoundCords
```List<Vector3> RoundCords()```
```List<Vector2> RoundCords()```
Rounds all vectors in the list using ```RoundCords()```
#### GetString
```string GetString<T>(string separator = ", ")```<br>
Joins all IEnumerable<T> elements to string with separator.
#### Next
```T Next<T>(T current)```<br>
Returns next element from IEnumerable<T> sequence.
#### Nearest
```T Nearest<T>(IEnumerable<T> list, Vector3 nearestTo)```<br>
```Vector3 Nearest<Vector3>(IEnumerable<Vector3> list, Vector3 nearestTo)```<br>
Returns nearest element from IEnumerable<T> list to given Vector3. T must inherit from Component.
<br><br><br>
## GameObjectExtension <a name="GameObjectExtension"></a>
#### GetOrAddComponent
```T GetOrAddComponent<T>() where T : Component```<br>
Returns the component of Type type. If one doesn't already exist on the GameObject it will be added.
#### AddComponentCopy
```T AddComponentCopy<T>() where T : Component```<br>
Adds a new copy of the provided component to a gameObject and returns it.
#### GetPrefabName
```string GetPrefabName<T>() where T : Component```<br>
```string GetPrefabName()```<br>
Returns prefab name.
<br><br><br>
## MonoBehaviourExtension <a name="MonoBehaviourExtension"></a>
#### SetActiveGO
```void SetActiveGO<T>(bool flag) where T : Component```<br>
Sets the active state of the GameObject.
#### ToggleActiveGO
```void ToggleActiveGO<T>() where T : Component```<br>
Toggles the active state of the GameObject.
<br><br><br>
## RectExtension <a name="RectExtension"></a>
#### IsOverlapsingOther
```bool IsOverlapsingOther(RectTransform b)```<br>
```bool IsOverlapsingOther(RectTransform b, bool allowInverse)```<br>
Returns true if a and b are overlapping.
#### WorldRect
```Rect WorldRect()```<br>
Calculate the world rect of a RectTransform.
#### SetToTextHeight
```GameObject SetToTextHeight()```<br>
Sets rectTransform's size to TextMeshProUGUI's or regular Text's preferred height.
<br><br><br>
## RendererExtension <a name="RendererExtension"></a>
#### Flash
```void Flash(Color color, Color returnColor, float time = 0.3f, Action callback = null)```<br>
Flashes renderer with given color for given time. Gives back to returnColor color after time. 
TODO: lerb color
<br><br><br>
## TransformExtension <a name="TransformExtension"></a>
#### FindChildByName
```Transform FindChildByName(string name)```<br>
Returns child of parent with given name. Equals to Utils.FindChild(parent, name).
#### DistanceXZ
```float DistanceXZ(Transform other)```<br>
```float DistanceXZ(Component other)```<br>
```float DistanceXZ(GameObject other)```<br>
```float DistanceXZ(Vector3 other)```<br>
Equals to Utils.DistanceXZ(Vector3 v0, Vector3 v1).
