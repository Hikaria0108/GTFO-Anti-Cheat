namespace Hikaria.GTFO_Anti_Cheat.Lang
{
    internal class English : LanguageBase
    {
        public override string WORKING_MESSAGE
        {
            get
            {
                return "<#F80>[GTFO_Anti-Cheat] Is now functioning, anti-cheating is only avaliable on host";
            }
        }

        public override string COMMAND_LIST
        {
            get
            {
                return "<#F80>[GTFO_Anti-Cheat] Enter /gac help to list all commands";
            }
        }

        public override string CURRENT_VERSION
        {
            get
            {
                return "<#F80>[GTFO_Anti-Cheat] Build v{0}";
            }
        }

        public override string CHECKING_UPDATE
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] Checking for new updates...";
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
                return "<#F00>[GTFO_Anti-Cheat] Anti-cheating will not function since you are not the host";
            }
        }

        public override string IS_HOST
        {
            get
            {
                return "<#0F0>[GTFO_Anti-Cheat] Anti-cheating will function since you are the host";
            }
        }

        public override string ILLEGAL_PLUGIN
        {
            get
            {
                return "<#F00>[GTFO_Anti-Cheat] Illegal plugin(s) has been detected and anti-cheating has been disabled";
            }
        }
        public override string ENVIRONMENT_ERROR
        {
            get
            {
                return "<#F00>[GTFO_Anti-Cheat] Environment anomaly detected, anti-cheat has been disabled";
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
                return "<#F80>[GTFO Anti-Cheat] <#0F0>Currently the latest version, build v{0}";
            }
        }

        public override string NEW_VERSION_DETECTED
        {
            get
            {
                return "<#F80>[GTFO Anti-Cheat] New version detected v{0}";
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
                return "<#F80>[GTFO Anti-Cheat] Banned player {0}[{1}] was refused to join lobby";
            }
        }
    }
}
