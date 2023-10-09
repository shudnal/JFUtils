using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using ServerSync;

namespace JFUtils.Valheim;

public sealed class ModBase
{
    public static string ModName, ModAuthor, ModVersion, ModGUID;
    public readonly Harmony harmony;
    public static ModBase mod;
    public static Action OnConfigurationChanged;
    public static AssetBundle bundle;

    private static BaseUnityPlugin plugin;
    private static bool sendDebugMessagesToHud;
    private static ConfigEntry<bool> sendDebugMessagesToHudConfig;

    public AssetBundle LoadAssetBundle(string filename)
    {
        var assembly = Assembly.GetExecutingAssembly();

        string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(filename));

        using Stream stream = assembly.GetManifestResourceStream(resourceName)!;
        bundle = AssetBundle.LoadFromStream(stream);
        return bundle;
    }

    private ModBase(BaseUnityPlugin _plugin, string modName, string modAuthor, string modVersion, bool
        pathAll =
        true)
    {
        if (mod)
            throw new Exception($"Trying to create new mod {modName}, but {ModName} already exists");

        ModName = modName;
        ModAuthor = modAuthor;
        ModVersion = modVersion;
        ModGUID = $"com.{ModAuthor}.{ModName}";
        harmony = new Harmony(ModGUID);
        ConfigFileName = $"{ModGUID}.cfg";
        plugin = _plugin;
        mod = this;
        bundle = null;

        configSync = new ConfigSync(ModName)
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };
        plugin.Config.SaveOnConfigSet = false;
        SetupWatcher();
        plugin.Config.ConfigReloaded += (_, _) => UpdateConfiguration();

        serverConfigLocked = config("General", "ServerConfigLock", false, "");
        configSync.AddLockingConfigEntry(serverConfigLocked);
        sendDebugMessagesToHudConfig = config("Debug", "Send debug messages to hud", true, "");

        plugin.Config.SaveOnConfigSet = true;
        plugin.Config.Save();
        if (pathAll) harmony.PatchAll();
    }

    public static ModBase CreateMod(BaseUnityPlugin _plugin, string modName, string modAuthor, string modVersion,
        bool pathAll = true) =>
        new(_plugin, modName, modAuthor, modVersion, pathAll);

    private static readonly Type pluginType = typeof(BaseUnityPlugin);

    public static T GetPlugin<T>() where T : BaseUnityPlugin => (T)plugin;
    public static BaseUnityPlugin GetPlugin() => plugin;

    public static implicit operator bool(ModBase modBase) => modBase != null;

    #region Debug

    public static void Debug(object msg, bool showInHud = false, bool showInConsole = false)
    {
        plugin.Logger.LogInfo(msg);
        msg = $"[{ModName}] {msg}";
        if (showInConsole && Console.IsVisible()) Console.instance.AddString(msg.ToString());
        if (showInHud && Player.m_localPlayer && Player.m_debugMode && sendDebugMessagesToHud)
            Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, msg.ToString());
    }

    public static void DebugError(object msg, bool showWriteToDev = true)
    {
        if (Console.IsVisible()) Console.instance.AddString($"<color=red>[{ModName}] {msg}</color>");
        if (showWriteToDev) msg += "Write to the developer and moderator if this happens often.";
        plugin.Logger.LogError(msg);
    }

    public static void DebugWarning(object msg, bool showWriteToDev = true)
    {
        if (Console.IsVisible()) Console.instance.AddString($"<color=yellow>[{ModName}] {msg}</color>");
        if (showWriteToDev) msg += "Write to the developer and moderator if this happens often.";
        plugin.Logger.LogWarning(msg);
    }

    #endregion

    #region ConfigSettings

    #region Core

    private readonly string ConfigFileName = "-1";
    private DateTime LastConfigChange;
    public ConfigSync configSync;
    private static ConfigEntry<bool> serverConfigLocked = null!;

    public ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
        bool synchronizedSetting = true)
    {
        var configEntry = plugin.Config.Bind(group, name, value, description);

        var syncedConfigEntry = configSync.AddConfigEntry(configEntry);
        syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

        return configEntry;
    }

    public ConfigEntry<T> config<T>(string group, string name, T value, string description,
        bool synchronizedSetting = true)
    {
        return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
    }

    public enum Toggle
    {
        On = 1,
        Off = 0
    }

    #endregion

    #region Updating

    private void SetupWatcher()
    {
        FileSystemWatcher fileSystemWatcher = new(Paths.ConfigPath, ConfigFileName);
        fileSystemWatcher.Changed += ConfigChanged;
        fileSystemWatcher.IncludeSubdirectories = true;
        fileSystemWatcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
        fileSystemWatcher.EnableRaisingEvents = true;
    }

    private void ConfigChanged(object sender, FileSystemEventArgs e)
    {
        if ((DateTime.Now - LastConfigChange).TotalSeconds <= 2) return;
        LastConfigChange = DateTime.Now;

        try
        {
            plugin.Config.Reload();
        }
        catch
        {
            DebugError("Unable reload config");
        }
    }

    private void UpdateConfiguration()
    {
        try
        {
            sendDebugMessagesToHud = sendDebugMessagesToHudConfig.Value;
            OnConfigurationChanged?.Invoke();
            if (OnConfigurationChanged == null) Debug("Configuration Received");
        }
        catch (Exception e)
        {
            DebugError($"Configuration error: {e.Message}", false);
        }
    }

    #endregion

    #endregion

    public void RunCommand(Terminal.ConsoleEvent action, Terminal.ConsoleEventArgs args)
    {
        try
        {
            action(args);
        }
        catch (ConsoleException e)
        {
            if (e is ConsoleException) args.Context.AddString("<color=red>Error: " + e.Message + "</color>");
            else DebugError(e);
        }
    }

    public bool IsAdmin => configSync.IsAdmin || (ZNet.instance && ZNet.instance.IsServer());
}