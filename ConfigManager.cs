using System.IO;
using BepInEx;
using BepInEx.Configuration;
using Hikaria.GTFO_Anti_Cheat.Lang;
using Hikaria.GTFO_Anti_Cheat.Utils;

namespace Hikaria.GTFO_Anti_Cheat
{
    internal class ConfigManager
    {
        static ConfigManager()
        {
            Logs.LogDebug("正在加载配置文件...");
            ConfigFile configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "Hikaria.GTFO_Anti-Cheat.cfg"), true);
            ConfigManager.language = configFile.Bind<string>(ConfigDescription.COMMON_SETTINGS, ConfigDescription.LANGUAGE_NAME, "zh-cn", ConfigDescription.LANGUAGE_DESC);
            ConfigManager.disableEnvironmentDetect = configFile.Bind<bool>(ConfigDescription.SAFE_SETTINGS, ConfigDescription.DISABLE_ENVIRONMENT_DETECT_NAME, false, ConfigDescription.DISABLE_ENVIRONMENT_DETECT_DESC);
            ConfigManager.detectBoosterHack = configFile.Bind<bool>(ConfigDescription.DETECT_SETTINGS, ConfigDescription.DETECT_BOOSTER_HACK_NAME, true, ConfigDescription.DETECT_BOOSTER_HACK_DESC);
            ConfigManager.autoKickPlayer = configFile.Bind<bool>(ConfigDescription.PLAYER_SETTINGS, ConfigDescription.AUTO_KICK_CHEATER_NAME, false, ConfigDescription.AUTO_KICK_CHEATER_DESC);
            ConfigManager.autoBanPlayer = configFile.Bind<bool>(ConfigDescription.PLAYER_SETTINGS, ConfigDescription.AUTO_BAN_CHEATER_NAME, false, ConfigDescription.AUTO_BAN_CHEATER_DESC);
            Logs.LogDebug("配置文件加载完成");
        }

        public static ConfigEntry<string> language;

        public static ConfigEntry<bool> disableEnvironmentDetect;

        public static ConfigEntry<bool> autoKickPlayer;

        public static ConfigEntry<bool> autoBanPlayer;

        public static ConfigEntry<bool> detectBoosterHack;

        public LanguageBase Language
        {
            get
            {
                string value = ConfigManager.language.Value;
                if (!(value == "zh-cn"))
                {
                    if (value == "en-us")
                    {
                        return new English();
                    }
                }
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
    }
}
