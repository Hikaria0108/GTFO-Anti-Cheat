using System.IO;
using BepInEx;
using BepInEx.Configuration;
using Hikaria.GTFO_Anti_Cheat.Lang;
using Hikaria.GTFO_Anti_Cheat.Utils;

namespace Hikaria.GTFO_Anti_Cheat.Managers
{
    internal class ConfigManager
    {
        static ConfigManager()
        {
            Logs.LogDebug("Loading config...");
            ConfigFile configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "Hikaria.GTFO_Anti-Cheat.cfg"), true);

            ConfigManager.language = configFile.Bind<string>(ConfigDescription.COMMON_SETTINGS, ConfigDescription.LANGUAGE_NAME, "zh-cn", ConfigDescription.LANGUAGE_DESC);
            ConfigManager.enableBroadcast = configFile.Bind<bool>(ConfigDescription.COMMON_SETTINGS, ConfigDescription.ENABLE_BROADCAST_NAME, true, ConfigDescription.ENABLE_BROADCAST_DESC);

            ConfigManager.disableEnvironmentDetect = configFile.Bind<bool>(ConfigDescription.SAFE_SETTINGS, ConfigDescription.DISABLE_ENVIRONMENT_DETECT_NAME, false, ConfigDescription.DISABLE_ENVIRONMENT_DETECT_DESC);

            ConfigManager.autoKickPlayer = configFile.Bind<bool>(ConfigDescription.PLAYER_SETTINGS, ConfigDescription.AUTO_KICK_CHEATER_NAME, false, ConfigDescription.AUTO_KICK_CHEATER_DESC);
            ConfigManager.autoBanPlayer = configFile.Bind<bool>(ConfigDescription.PLAYER_SETTINGS, ConfigDescription.AUTO_BAN_CHEATER_NAME, false, ConfigDescription.AUTO_BAN_CHEATER_DESC);
            ConfigManager.loadOnlinePlayerLists = configFile.Bind<bool>(ConfigDescription.PLAYER_SETTINGS, ConfigDescription.LOAD_ONLINE_PLAYER_LISTS_NAME, true, ConfigDescription.LOAD_ONLINE_PLAYER_LISTS_DESC);

            ConfigManager.detectBoosterHack = configFile.Bind<bool>(ConfigDescription.DETECT_SETTINGS, ConfigDescription.DETECT_BOOSTER_HACK_NAME, true, ConfigDescription.DETECT_BOOSTER_HACK_DESC);
            ConfigManager.detectWeaponModelHack = configFile.Bind<bool>(ConfigDescription.DETECT_SETTINGS, ConfigDescription.DETECT_WEAPON_MODEL_HACK_NAME, true, ConfigDescription.DETECT_WEAPON_MODEL_HACK_DESC);
            ConfigManager.detectWeaponDataHack = configFile.Bind<bool>(ConfigDescription.DETECT_SETTINGS, ConfigDescription.DETECT_WEAPON_DATA_HACK_NAME, true, ConfigDescription.DETECT_WEAPON_DATA_HACK_DESC);

            Logs.LogDebug("Config loaded");
        }

        public static readonly ConfigEntry<string> language;

        public static readonly ConfigEntry<bool> disableEnvironmentDetect;

        public static readonly ConfigEntry<bool> autoKickPlayer;

        public static readonly ConfigEntry<bool> autoBanPlayer;

        public static readonly ConfigEntry<bool> detectBoosterHack;

        public static readonly ConfigEntry<bool> loadOnlinePlayerLists;

        public static readonly ConfigEntry<bool> enableBroadcast;

        public static readonly ConfigEntry<bool> detectWeaponModelHack;

        public static readonly ConfigEntry<bool> detectWeaponDataHack;

        public LanguageBase Language
        {
            get
            {
                string value = ConfigManager.language.Value;
                if (!(value == "zh-cn"))
                {
                    if (value == "en-us")
                    {
                        EntryPoint.IsEnglish = true;
                        return new English();
                    }
                }
                EntryPoint.IsEnglish = false;
                return new SimplifiedChinese();
            }
        }

        public bool AutoKickPlayer
        {
            get
            {
                return autoKickPlayer.Value;
            }
        }

        public bool AutoBanPlayer
        {
            get
            {
                return autoBanPlayer.Value;
            }
        }

        public bool DetectBoosterHack
        {
            get
            {
                return detectBoosterHack.Value; 
            }
        }

        public bool DetectEnvironmentDetect
        {
            get
            {
                return disableEnvironmentDetect.Value;
            }
        }

        public bool LoadOnlinePlayerLists
        {
            get
            {
                return loadOnlinePlayerLists.Value;
            }
        }

        public bool EnableBroadcast
        {
            get
            {
                return enableBroadcast.Value;
            }
        }

        public bool DetectWeaponDataHack
        {
            get
            {
                return detectWeaponDataHack.Value;
            }
        }

        public bool DetectWeaponModelHack
        {
            get
            {
                return detectWeaponModelHack.Value;
            }
        }
    }
}
