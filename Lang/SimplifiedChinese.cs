using System.Runtime.InteropServices.WindowsRuntime;

namespace Hikaria.GTFO_Anti_Cheat.Lang
{
    internal class SimplifiedChinese : LanguageBase
    {
        public override string WORKING_MESSAGE
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> 现已工作，本mod仅限房主有效";
            }
        }

        public override string COMMAND_LIST
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> /achelp 可查看所有指令";
            }
        }

        public override string CURRENT_VERSION
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> 版本v{0}";
            }
        }

        public override string CHECKING_UPDATE
        {
            get
            {
                return "开始检查更新...";
            }
        }

        public override string LOADED
        {
            get
            {
                return "GTFO_Anti-Cheat 已加载";
            }
        }

        public override string IGNORE_REPEAT_PATCH
        {
            get
            {
                return "忽略重复的补丁: {0}";
            }
        }

        public override string PATCHING
        {
            get
            {
                return "正在应用补丁: {0}";
            }
        }

        public override string NOT_HOST
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> <color=red>你当前不是房主，反作弊已禁用</color>";
            }
        }

        public override string IS_HOST
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> <color=green>你当前是房主，反作弊已启用</color>";
            }
        }

        public override string ILLEGAL_PLUGIN
        {
            get
            {
                return "检测到已加载非法插件，反作弊已禁用";
            }
        }

        public override string ENVIRONMENT_ERROR
        {
            get
            {
                return "检测环境异常，反作弊已禁用";
            }
        }

        public override string ANTI_CHEAT_BROADCAST
        {
            get
            {
                return "<color=orange>本房间已启用 GTFO Anti-Cheat</color>";
            }
        }

        public override string BOOSTER_HACK
        {
            get
            {
                return "修改强化剂";
            }
        }

        public override string BAN
        {
            get
            {
                return "踢出并封禁";
            }
        }

        public override string KICK
        {
            get
            {
                return "踢出";
            }
        }

        public override string CHEATER_DETECTED_MESSAGE
        {
            get
            {
                return "<#F80>[GTFO Auti-Cheat] 检测到作弊玩家: {0}";
            }
        }

        public override string CHEATING_BEHAVIOR_MESSAGE
        {
            get
            {
                return "<#F80>[GTFO Auti-Cheat] 作弊行为: {0}";
            }
        }
    }
}
