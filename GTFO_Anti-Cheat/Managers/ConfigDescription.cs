namespace Hikaria.GTFO_Anti_Cheat.Managers
{
    internal class ConfigDescription
    {
        internal static string LANGUAGE_DESC
        {
            get
            {
                return "切换显示语言，接受的值：[zh-cn|en-us]";
            }
        }

        internal static string COMMON_SETTINGS
        {
            get
            {
                return "通用设置";
            }
        }

        internal static string PLAYER_SETTINGS
        {
            get
            {
                return "玩家设置";
            }
        }

        internal static string LANGUAGE_NAME
        {
            get
            {
                return "Language";
            }
        }

        internal static string AUTO_KICK_CHEATER_NAME
        {
            get
            {
                return "AutoKickPlayer";
            }
        }

        internal static string AUTO_KICK_CHEATER_DESC
        {
            get
            {
                return "自动踢出作弊玩家，接受的值：[true|false]";
            }
        }

        internal static string AUTO_BAN_CHEATER_NAME
        {
            get
            {
                return "AutoBanPlayer";
            }
        }

        internal static string AUTO_BAN_CHEATER_DESC
        {
            get
            {
                return "自动踢出并封禁作弊玩家，接受的值：[true|false]";
            }
        }

        internal static string DETECTOR_SETTINGS
        {
            get
            {
                return "作弊检测设置";
            }
        }
        internal static string DETECT_BOOSTER_HACK_NAME
        {
            get
            {
                return "DetectBoosterHack";
            }
        }

        internal static string DETECT_BOOSTER_HACK_DESC
        {
            get
            {
                return "启用强化剂数据检测，接受的值：[true|false]";
            }
        }

        internal static string DISABLE_ENVIRONMENT_DETECT_NAME
        {
            get
            {
                return "DisableEnvironmentDetect";
            }
        }

        internal static string DISABLE_ENVIRONMENT_DETECT_DESC
        {
            get
            {
                return "禁用游戏环境（修改器、非法插件等）检测，接受的值：[true|false]";
            }
        }

        internal static string SAFE_SETTINGS
        {
            get
            {
                return "安全设置";
            }
        }

        internal static string LOAD_ONLINE_PLAYER_LISTS_NAME
        {
            get
            {
                return "EnableOnlinePlayerLists";
            }
        }

        internal static string LOAD_ONLINE_PLAYER_LISTS_DESC
        {
            get
            {
                return "启用在线玩家黑白名单，接受的值：[true|false]";
            }
        }

        internal static string ENABLE_BROADCAST_NAME
        {
            get
            {
                return "EnableBroadcast";
            }
        }

        internal static string ENABLE_BROADCAST_DESC
        {
            get
            {
                return "启用玩家作弊通报，接受的值：[true|false]";
            }
        }

        internal static string DETECT_WEAPON_DATA_HACK_DESC
        {
            get
            {
                return "启用武器数据检测，接受的值：[true|false]";
            }
        }

        internal static string DETECT_WEAPON_DATA_HACK_NAME
        {
            get
            {
                return "DetectWeaponDataHack";
            }
        }
    }
}
