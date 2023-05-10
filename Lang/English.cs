namespace Hikaria.GTFO_Anti_Cheat.Lang
{
    internal class English : LanguageBase
    { 
        public override string COMMAND_LIST
        {
            get
            {
                return "<#F80>[GTFO_Anti-Cheat] Enter /gac help to list all commands";
            }
        }

        public override string CHECKING_UPDATE
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] Checking for updates...";
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
                return "<#F80>[GTFO_Anti-Cheat] <#F00>Anti-cheating will not function since you are not the host";
            }
        }

        public override string IS_HOST
        {
            get
            {
                return "<#F80>[GTFO_Anti-Cheat] <#0F0>Anti-cheating will function since you are the host";
            }
        }

        public override string ILLEGAL_PLUGIN
        {
            get
            {
                return "<#F80>[GTFO_Anti-Cheat] <#F00>Illegal plugin(s) has been detected and anti-cheating has been disabled";
            }
        }
        public override string ENVIRONMENT_ERROR
        {
            get
            {
                return "<#F80>[GTFO_Anti-Cheat] <#F00>Environment anomaly detected, anti-cheat has been disabled";
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
                return "<#F00>[GTFO Anti-Cheat] Check update failure!";
            }
        }

        public override string IS_LATEST_VERSION
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] <#0F0>Currently is the latest version, build v{0}";
            }
        }

        public override string NEW_VERSION_DETECTED
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] Update detected v{0}";
            }
        }

        public override string ONLINE_PLAYER_LISTS_LOADING
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] Loading online player whitelist and blacklist...";
            }
        }

        public override string ONLINE_PLAYER_LISTS_LOADED
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] Online player whitelist and blacklist loaded";
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
                return "<#F80>[GTFO Anti-Cheat] ChangeLog:";
            }
        }

        public override string BANNED_PLAYER_WAS_REFUSED_TO_JOIN_LOBBY
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] {0} banned player {1}[{2}] was refused to join lobby";
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
                return "<#0F0>Player [{0}] has been unbanned locally";
            }
        }
    }
}
