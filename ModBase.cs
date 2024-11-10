﻿using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using JFUtils.Valheim.WithPatch;
using JFUtils.WithPatch;
using ServerSync;
using Steamworks;
using UnityEngine.Networking;

namespace JFUtils;

[PublicAPI]
public static class ModBase
{
    public static string CreateModGUID(string ModName, string ModAuthor) => $"com.{ModAuthor}.{ModName}";
    public static string ModName { get; private set; }
    public static string ModAuthor { get; private set; }
    public static string ModVersion { get; private set; }
    public static string ModGUID { get; private set; }

    public static Harmony harmony { get; private set; }
    public static Action OnConfigurationChanged;
    public static AssetBundle bundle;

    private static BaseUnityPlugin plugin;
    private static bool sendDebugMessagesToHud;
    private static ConfigEntry<bool> sendDebugMessagesToHudConfig;

    public static void EnableObjectsInstances() => ObjectsInstances.enabled = true;
    public static void EnableImportantZDOs() => ImportantZDOs.enabled = true;

    public static AssetBundle LoadAssetBundle(string filename)
    {
        var assembly = Assembly.GetExecutingAssembly();
        string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(filename));

        using (Stream stream = assembly.GetManifestResourceStream(resourceName)!)
            bundle = AssetBundle.LoadFromStream(stream);
        return bundle;
    }

    public static void CreateMod(BaseUnityPlugin _plugin, string modName, string modAuthor,
        string modVersion, string modGUID, bool pathAll = true)
    {
        plugin = _plugin;
        ModName = modName;
        ModAuthor = modAuthor;
        ModVersion = modVersion;

        //TODO: auto grab modName, modAuthor, modVersion and modGUID

        ModGUID = CreateModGUID(ModName, ModAuthor);
        if (modGUID != ModGUID)
        {
            DebugError(
                $"Mod GUID doesn't match required format: com.ModAuthor.ModName - Got: {modGUID}, expected: {ModGUID}");
            ModGUID = modGUID;
        }

        harmony = new Harmony(ModGUID);
        ConfigFileName = ModGUID + ".cfg";
        bundle = null;

        configSync = new ConfigSync(ModName)
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };
        plugin.Config.SaveOnConfigSet = false;
        SetupWatcher();
        plugin.Config.ConfigReloaded += (_, _) => UpdateConfiguration();

        serverConfigLocked = config("General", "ServerConfigLock", true,
            "Locks client config file so it can't be modified");
        configSync.AddLockingConfigEntry(serverConfigLocked);
        sendDebugMessagesToHudConfig = config("Debug", "Send debug messages to hud", true, "");

        plugin.Config.SaveOnConfigSet = true;
        plugin.Config.Save();
        if (pathAll) harmony.PatchAll();
        else
        {
            harmony.PatchAll(typeof(GlobalRPCs));
            harmony.PatchAll(typeof(ImportantZDOs));
            harmony.PatchAll(typeof(RegisterObjectsInstances));
            harmony.PatchAll(typeof(UpdateConfigOnGameStart));
            harmony.PatchAll(typeof(ZoneSystemExtension_Patch));
        }
    }

    public static T GetPlugin<T>() where T : BaseUnityPlugin => (T)plugin;
    public static BaseUnityPlugin GetPlugin() => plugin;

    public static void RegisterImportantZDO(ImportantZDO_Settings ZDO_settings) =>
        ImportantZDOs.RegisterImportantZDO(ZDO_settings);

    public static void LoadImageFromWEB(string url, Action<Sprite> callback)
    {
        if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out _)) return;

        GetPlugin().StartCoroutine(LoadImage_IEnumerator(url, callback));
    }

    private static IEnumerator LoadImage_IEnumerator(string url, Action<Sprite> callback)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.result is UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            if (texture.width == 0 || texture.height == 0) yield break;
            Texture2D temp = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            temp.SetPixels(texture.GetPixels());
            temp.Apply();
            var sprite = Sprite.Create(texture, new(0, 0, texture.width, texture.height), new(0.5f, 0.5f));
            callback?.Invoke(sprite);
        }
    }

    #region Debug

    public static void Debug(object msg, bool showInHud = false, bool showInConsole = false)
    {
        ZLog.Log($"[{ModName}]: {msg}");
        msg = $"[{ModName}] {msg}";
        if (showInConsole && Console.IsVisible()) Console.instance.AddString(msg.ToString());
        if (showInHud && Player.m_localPlayer && Player.m_debugMode && sendDebugMessagesToHud)
            Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, msg.ToString());
    }

    public static void DebugError(object msg, bool showWriteToDev = false, bool showInConsole = false)
    {
        if (showInConsole && Console.IsVisible())
            Console.instance.AddString($"<color=F8733C>[{ModName}] {msg}</color>");
        if (showWriteToDev) msg += " Write to the developer and moderator if this happens often.";
        ZLog.LogError($"[{ModName}]: {msg}");
    }

    public static void DebugWarning(object msg, bool showWriteToDev = false, bool showInConsole = false)
    {
        if (showInConsole && Console.IsVisible())
            Console.instance.AddString($"<color=yellow>[{ModName}] {msg}</color>");
        if (showWriteToDev) msg += " Write to the developer and moderator if this happens often.";
        ZLog.LogWarning($"[{ModName}]: {msg}");
    }

    #endregion

    #region ConfigSettings

    #region Core

    private static string ConfigFileName = "-1";
    private static DateTime LastConfigChange;
    public static ConfigSync configSync;
    private static ConfigEntry<bool> serverConfigLocked = null!;

    public static ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
        bool synchronizedSetting = true)
    {
        ConfigDescription extendedDescription =
            new(
                description.Description +
                (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                description.AcceptableValues, description.Tags);
        var configEntry = plugin.Config.Bind(group, name, value, extendedDescription);

        var syncedConfigEntry = configSync.AddConfigEntry(configEntry);
        syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

        return configEntry;
    }

    public static ConfigEntry<T> config<T>(string group, string name, T value, string description,
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

    private static void SetupWatcher()
    {
        FileSystemWatcher fileSystemWatcher = new(Paths.ConfigPath, ConfigFileName);
        fileSystemWatcher.Changed += ConfigChanged;
        fileSystemWatcher.IncludeSubdirectories = true;
        fileSystemWatcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
        fileSystemWatcher.EnableRaisingEvents = true;
    }

    private static void ConfigChanged(object sender, FileSystemEventArgs e)
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

    private static void UpdateConfiguration()
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

    public static void RunCommand(Terminal.ConsoleEvent action, Terminal.ConsoleEventArgs args)
    {
        try
        {
            action(args);
        }
        catch (ConsoleCommandException e)
        {
            args.Context.AddString("<color=red>Error: " + e.Message + "</color>");
        }
        catch (Exception e)
        {
            DebugError(e, showInConsole: false);
        }
    }

    public static ulong GetSteamID() => SteamUser.GetSteamID().m_SteamID;

    public static bool IsAdmin => configSync.IsAdmin || (ZNet.instance && ZNet.instance.IsServer());
}