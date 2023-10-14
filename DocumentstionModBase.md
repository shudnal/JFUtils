## ModBase
All methods, fields and properties are static.
### Methods
#### CreateMod()
```void CreateMod(BaseUnityPlugin _plugin, string modName, string modAuthor, string modVersion, bool pathAll = true)```
Allows you to create your own mod automatically.
Call ```ModBase.CreateMod()``` to create a mod. It will automatically:
- create ConfigSync
- SetupWatcher on config file
- call Harmony.PatchAll() - can be disabled via ```pathAll: false```
- Add config server lock
```csharp
 private void Awake()
    {
        ModBase.CreateMod(_plugin: this, ModName, ModAuthor, ModVersion, pathAll: true);
        OnConfigurationChanged += () => {  };
    }
```
#### RunCommand
```void RunCommand(Terminal.ConsoleEvent action, Terminal.ConsoleEventArgs args)```
When you adding a new command, you can call this method to format it. It automatically uses try catch.
throw new ConsoleCommandException to make error be shown only in in-game console. Any other exception will be shown in the log.
Example:
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
```T GetPlugin<T>() where T : BaseUnityPlugin```
Returns the plugin instance of type of BaseUnityPlugin.
#### T GetPlugin<T>()
Returns the plugin instance of type of T. T should your BaseUnityPlugin type.
```csharp
GetPlugin<MyMod>().SomeMethodImplodedInMyMod();
```
#### Debug
```void Debug(object msg, bool showInHud = false, bool showInConsole = false)```
Logs message to the BepInEx console.
#### DebugError
```void DebugError(object msg, bool showWriteToDev = true, bool showInConsole = false)```
Logs error message to the BepInEx console.
#### DebugWarning
```void DebugWarning(object msg, bool showWriteToDev = true, bool showInConsole = false)```
Logs warning message to the BepInEx console.
#### DebugWarning
```AssetBundle LoadAssetBundle(string filename)```
Loads an asset bundle, returns the it and saves it in ```ModBase.bundle``` field.
#### CreateModGUID
```string CreateModGUID(string ModName, string ModAuthor) => $"com.{ModAuthor}.{ModName}";```
Constructs a mod GUID. in format ```com.ModAuthor.ModName```.

### Properties
#### ModName
```string ModName { get; private set; }```
The name of the mod.
#### ModAuthor
```string ModAuthor { get; private set; }```
The author of the mod.
#### ModVersion
```string ModVersion { get; private set; }```
The version of the mod.
#### ModGUID
```string ModGUID { get; private set; }```
The GUID of the mod.
#### Harmony
```Harmony harmony { get; private set; }```
The harmony instance for this mod.
#### IsAdmin
```bool IsAdmin```
Whether the player is an admin on this server.

### Fields
#### bundle
```AssetBundle bundle```
The asset bundle of the mod set by ```LoadAssetBundle```.

### Events
#### OnConfigurationChanged
```Action OnConfigurationChanged```
Called when configuration file is changed.
