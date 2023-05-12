namespace Hikaria.GTFO_Anti_Cheat.Lang
{
    internal class English : LanguageBase
    { 
        public override string COMMAND_UNKNOWN_HELP
        {
            get
            {
                return "<color=orange>[GTFO_Anti-Cheat]</color> <#F00>Unknown Command, enter /gac help to list all commands";
            }
        }

        public override string CHECKING_UPDATE
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> Checking for updates...";
            }
        }

        public override string LOADED
        {
            get
            {
                return "GTFO_Anti-Cheat Loaded";
            }
        }

        public override string IGNORE_REPEAT_PATCH
        {
            get
            {
                return "Ignoring duplicate patch: {0}";
            }
        }

        public override string PATCHING
        {
            get
            {
                return "Applying patch: {0}";
            }
        }

        public override string NOT_HOST
        {
            get
            {
                return "<color=orange>[GTFO_Anti-Cheat]</color> <#F00>Anti-cheating will not function since you are not the host";
            }
        }

        public override string IS_HOST
        {
            get
            {
                return "<color=orange>[GTFO_Anti-Cheat]</color> <#0F0>Anti-cheating will function since you are the host";
            }
        }

        public override string ILLEGAL_PLUGIN
        {
            get
            {
                return "<color=orange>[GTFO_Anti-Cheat]</color> <#F00>Illegal plugin(s) has been detected and anti-cheating has been disabled";
            }
        }
        public override string ENVIRONMENT_ERROR
        {
            get
            {
                return "<color=orange>[GTFO_Anti-Cheat]</color> <#F00>Environment anomaly detected, anti-cheat has been disabled";
            }
        }

        public override string ANTI_CHEAT_BROADCAST
        {
            get
            {
                return "<#F80>[GTFO_Anti-Cheat] Anti-Cheat has been enabled for this game session";
            }
        }

        public override string BOOSTER_HACK
        {
            get
            {
                return "Modified boosters";
            }
        }

        public override string CHEATER_DETECTED_MESSAGE
        {
            get
            {
                return "<#F80>[GTFO Auti-Cheat] Cheating player detected: {0}";
            }
        }

        public override string CHEATING_BEHAVIOR_MESSAGE
        {
            get
            {
                return "<#F80>[GTFO Auti-Cheat] Cheating behavior: {0}";
            }
        }

        public override string CHECK_UPDATE_FAILD
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> <#F00>Check update failure!";
            }
        }

        public override string IS_LATEST_VERSION
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> <#0F0>Currently is the latest version, build v{0}";
            }
        }

        public override string NEW_VERSION_DETECTED
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> <#0F0>Update detected v{0}";
            }
        }

        public override string ONLINE_PLAYER_LISTS_LOADING
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> Loading online player whitelist and blacklist...";
            }
        }

        public override string ONLINE_PLAYER_LISTS_LOADED
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> Online player whitelist and blacklist loaded";
            }
        }

        public override string KICK_PLAYER
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] {0} was kicked from lobby.";
            }
        }

        public override string BAN_PLAYER
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] {0} was {1} kick from lobby for {2}.";
            }
        }

        public override string KICK_OR_BAN_REASON
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] Reason: {0}";
            }
        }

        public override string CHANGE_LOG
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> ChangeLog:";
            }
        }

        public override string BANNED_PLAYER_WAS_REFUSED_TO_JOIN_LOBBY
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> {0} banned player {1}[{2}] was refused to join lobby";
            }
        }

        public override string LOCAL_BANNED
        {
            get
            {
                return "Local";
            }
        }

        public override string ONLINE_BANNED
        {
            get
            {
                return "Online";
            }
        }

        public override string LOCAL_UNBAN_PLAYER_MESSAGE
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> Player [{0}] is removed from local blacklist";
            }
        }

        public override string WEAPON_MODEL_HACK 
        { 
            get
            {
                return "Modified weapon model";
            } 
        }

        public override string WEAPON_DATA_HACK
        {
            get
            {
                return "Modified weapon data";
            }
        }

        public override string TURN_ON
        {
            get
            {
                return "enabled";
            }
        }

        public override string TURN_OFF
        {
            get
            {
                return "disabled";
            }
        }

        public override string COMMAND_HINT_AUTOKICK
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> <color={0}>Auto kick cheaters {1}</color>";
            }
        }

        public override string COMMAND_HINT_AUTOBAN
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> <color={0}>Auto kick and ban cheaters {1}</color>";
            }
        }

        public override string COMMAND_HINT_BROADCAST
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> <color={0}>Detect cheaters broadcast {1}</color>";
            }
        }
        public override string COMMAND_HINT_DETECT_BOOSTER_HACK
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> <color={0}>Booster detector {1}</color>";
            }
        }
        public override string COMMAND_HINT_DETECT_WEAPON_DATA_HACK
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> <color={0}>Weapon data detector {1}</color>";
            }
        }

        public override string COMMAND_LIST
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> Avaliable commands:";
            }
        }

        public override string COMMAND_HELP
        {
            get
            {
                return "<color=orange>[GTFO Anti-Cheat]</color> Enter /gac help to list all commands";
            }
        }

        public override string COMMAND_DESC_AUTO_BAN_PLAYER
        {
            get
            {
                return "/gac autoban [on|off], enable or disable auto kick and ban cheaters";
            }
        }

        public override string COMMAND_DESC_AUTO_KICK_PLAYER
        {
            get
            {
                return "/gac autokick [on|off], enable or disable auto kick cheaters";
            }
        }

        public override string COMMAND_DESC_BROADCAST
        {
            get
            {
                return "/gac broadcast [on|off], enable or disable broadcast when cheaters detected";
            }
        }

        public override string COMMAND_DESC_DETECT_BOOSTER_HACK
        {
            get
            {
                return "/gac detect booster [on|off], enable or disable booster data detection";
            }
        }

        public override string COMMAND_DESC_DETECT_WEAPON_DATA_HACK
        {
            get
            {
                return "/gac detect weapondata [on|off], enable or disable weapon data detection";
            }
        }
    }
}
