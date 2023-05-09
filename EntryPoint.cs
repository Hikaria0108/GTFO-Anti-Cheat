using System;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using BepInEx;
using Hikaria.GTFO_Anti_Cheat.Lang;
using Player;
using Hikaria.GTFO_Anti_Cheat.Utils;
using Hikaria.GTFO_Anti_Cheat.Patches;
using Patch = Hikaria.GTFO_Anti_Cheat.Utils.Patch;
using Il2CppInterop.Runtime.Injection;

namespace Hikaria.GTFO_Anti_Cheat
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class EntryPoint : BasePlugin
    {
        public override void Load()
        {
            Instance = this;

            ConfigManager configManager = new ConfigManager();
            EntryPoint.Language = configManager.Language;
            EntryPoint.AutoBanPlayer = configManager.AutoBanPlayer;
            EntryPoint.AutoKickPlayer = configManager.AutoKickPlayer;
            EntryPoint.DetectBoosterHack = configManager.DetectBoosterHack;
            EntryPoint.DisableEnvironmentDetect = configManager.DetectEnvironmentDetect;

            if(!EntryPoint.DisableEnvironmentDetect)
            {
                if (DetectEnvironment())
                {
                    Logs.LogError(Language.ENVIRONMENT_ERROR);
                    this.Unload();
                }
            }
            
            RegisterTypesInIl2Cpp();
            RegisterPatchesInHarmony();

            Logs.LogMessage(EntryPoint.Language.LOADED);
        }

        public override bool Unload()
        {
            return base.Unload();
        }

        public static EntryPoint Instance;

        internal static Harmony s_harmonyInstance;

        internal static LanguageBase Language;

        private static bool MTFO_Installed;

        private static bool GTFO_API_Installed;

        internal static bool AutoKickPlayer;

        internal static bool AutoBanPlayer;

        internal static bool DetectBoosterHack;

        internal static bool DisableEnvironmentDetect;

        internal static bool EnableDebugInfo;

        private static bool DetectEnvironment()
        {
            BepInEx.PluginInfo pluginInfo;

            if (IL2CPPChainloader.Instance.Plugins.TryGetValue("dev.gtfomodding.gtfo-api", out pluginInfo))
            {
                EntryPoint.GTFO_API_Installed = true;
                Logs.LogError("GTFO-API Detected!");
            }

            if (IL2CPPChainloader.Instance.Plugins.TryGetValue("com.dak.MTFO", out pluginInfo))
            {
                EntryPoint.MTFO_Installed = true;
                Logs.LogError("MTFO Detected!");
            }

            if (GTFO_API_Installed || MTFO_Installed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void RegisterTypesInIl2Cpp()
        {
            ClassInjector.RegisterTypeInIl2Cpp<ChatManager>();
            ClassInjector.RegisterTypeInIl2Cpp<GameEventLogManager>();
            ClassInjector.RegisterTypeInIl2Cpp<LobbyManager>();
        }

        private static void RegisterPatchesInHarmony()
        {
            Patch.RegisterPatch<LocalPlayerPatch>();
            Patch.RegisterPatch<PlayerJoinLobby>();
            Patch.RegisterPatch<ChatBoxCommand>();
            Patch.RegisterPatch<DetectBoosterDataHack>();
            //Patch.RegisterPatch<DetectDamangeHack>();
        }
    }
}
