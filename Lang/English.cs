namespace Hikaria.GTFO_Anti_Cheat.Lang
{
    internal class English : LanguageBase
    {
        public override string WORKING_MESSAGE
        {
            get
            {
                return "<color=orange>[GTFO_Anti-Cheat]</color> Is now functioning, anti-cheating is only avaliable on host";
            }
        }

        public override string COMMAND_LIST
        {
            get
            {
                return "<color=orange>[GTFO_Anti-Cheat]</color> Enter /achelp to list all commands";
            }
        }

        public override string CURRENT_VERSION
        {
            get
            {
                return "<color=orange>[GTFO_Anti-Cheat]</color> Build v{0}";
            }
        }

        public override string CHECKING_UPDATE
        {
            get
            {
                return "Checking for new updates...";
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
                return "<color=red>[GTFO_Anti-Cheat] Anti-cheating will not function since you are not the host</color>";
            }
        }

        public override string IS_HOST
        {
            get
            {
                return "<color=green>[GTFO_Anti-Cheat] Anti-cheating will function since you are the host</color>";
            }
        }

        public override string ILLEGAL_PLUGIN
        {
            get
            {
                return "<color=red>[GTFO_Anti-Cheat] Illegal plugin(s) has been detected and anti-cheating has been disabled</color>";
            }
        }
        public override string ENVIRONMENT_ERROR
        {
            get
            {
                return "<color=red>[GTFO_Anti-Cheat] Environment anomaly detected, anti-cheat has been disabled</color>";
            }
        }

        public override string ANTI_CHEAT_BROADCAST
        {
            get
            {
                return "<color=orange>[GTFO_Anti-Cheat] GTFO Anti-Cheat has been enabled for this game session</color>";
            }
        }

        public override string BOOSTER_HACK
        {
            get
            {
                return "modified boosters";
            }
        }

        public override string BAN
        {
            get
            {
                return "kick and ban";
            }
        }

        public override string KICK
        {
            get
            {
                return "kick";
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
    }
}
