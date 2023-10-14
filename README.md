# JF Utils

Adds some usefull utils for programmers.
Includes ServerSync.dll inside
If you have any questions or suggestions please message me in disscord: ```justafrogger```

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
- - - [Methods](#modbase-methods)
- - - [Properties](#modbase-properties)
- - - [Events](#modbase-events)
- - - [Fields](#modbase-fields)
- - [TimeUtils](#TimeUtils)
- - [SimpleVector2](#SimpleVector2)
- - [SimpleVector3](#SimpleVector3)
- - [ObjectsInstances](#ObjectsInstances)
- - [ConsoleCommandException](#ConsoleCommandException)
- - [ConfigurationManagerAttributes](#ConfigurationManagerAttributes)
- Enums
- - [Wheather](#Wheather)
- Extensions
- - [ZoneSystemExtension](#ZoneSystemExtension)
- - [BiomeExtension](#BiomeExtension)
- - [ObjectDBExtension](#ObjectDBExtension)
- - [InventoryExtension](#InventoryExtension)
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
- - [RectTransformExtension](#RectTransformExtension)
- - [RendererExtension](#RendererExtension)
- - [TransformExtension](#TransformExtension)


## ModBase <a name="ModBase"></a>
All methods, fields and properties are static.
### Methods <a name="modbase-methods"></a>
#### CreateMod()
```void CreateMod(BaseUnityPlugin _plugin, string modName, string modAuthor, string modVersion, bool pathAll = true)```
<br>
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
Returns the plugin instance of type of BaseUnityPlugin.
#### T GetPlugin<T>()
Returns the plugin instance of type of T. T should your BaseUnityPlugin type.<br>
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

### Properties <a name="modbase-properties"></a>
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

### Fields <a name="modbase-fields"></a>
#### bundle
```AssetBundle bundle```<br>
The asset bundle of the mod set by ```LoadAssetBundle```.

### Events <a name="modbase-events"></a>
#### OnConfigurationChanged
```Action OnConfigurationChanged```<br>
Called when configuration file is changed.

