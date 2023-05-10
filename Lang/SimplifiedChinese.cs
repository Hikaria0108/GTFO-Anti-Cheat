using System.Runtime.InteropServices.WindowsRuntime;

namespace Hikaria.GTFO_Anti_Cheat.Lang
{
    internal class SimplifiedChinese : LanguageBase
    {
        public override string COMMAND_LIST
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] /gac help 可查看所有指令";
            }
        }

        public override string CHECKING_UPDATE
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] 开始检查更新...";
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
                return "<#F80>[GTFO Anti-Cheat] 本房间已启用反作弊";
            }
        }

        public override string BOOSTER_HACK
        {
            get
            {
                return "修改强化剂";
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

        public override string CHECK_UPDATE_FAILD
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] <#F00>检查更新失败！";
            }
        }

        public override string IS_LATEST_VERSION
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] <#0F0>当前版本已是最新版本 v{0}";
            }
        }

        public override string NEW_VERSION_DETECTED
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] <#0F0>检测到新版本 v{0}";
            }
        }

        public override string ONLINE_PLAYER_LISTS_LOADING
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] 正在加载在线玩家黑白名单...";
            }
        }

        public override string ONLINE_PLAYER_LISTS_LOADED
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] <#0F0>在线玩家黑白名单已加载";
            }
        }

        public override string KICK_PLAYER
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] 作弊玩家 {0} 被 踢出";
            }
        }

        public override string BAN_PLAYER
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] 作弊玩家 {0} 被 踢出并封禁";
            }
        }

        public override string KICK_OR_BAN_REASON
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] 原因: {0}";
            }
        }

        public override string CHANGE_LOG
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] 更新日志:";
            }
        }

        public override string BANNED_PLAYER_WAS_REFUSED_TO_JOIN_LOBBY
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] 被{0}封禁玩家 {1}[{2}] 被拒绝加入大厅";
            }
        }

        public override string LOCAL_BANNED
        {
            get
            {
                return "本地";
            }
        }

        public override string ONLINE_BANNED
        {
            get
            {
                return "在线";
            }
        }

        public override string LOCAL_UNBAN_PLAYER_MESSAGE
        {
            get
            {
                return "<#0F0>已为ID为 [{0}] 的玩家解除本地封禁";
            }
        }
    }
}
