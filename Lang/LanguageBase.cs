namespace Hikaria.GTFO_Anti_Cheat.Lang
{
    public abstract class LanguageBase
    {
        public abstract string COMMAND_LIST { get; }

        public abstract string CHECKING_UPDATE { get; }

        public abstract string LOADED { get; }

        public abstract string IGNORE_REPEAT_PATCH { get; }

        public abstract string NOT_HOST { get; }

        public abstract string IS_HOST { get; }

        public abstract string PATCHING { get; }

        public abstract string ILLEGAL_PLUGIN { get;}

        public abstract string ENVIRONMENT_ERROR { get; }

        public abstract string ANTI_CHEAT_BROADCAST { get; }

        public abstract string BOOSTER_HACK { get; }

        public abstract string CHEATER_DETECTED_MESSAGE { get;}

        public abstract string CHEATING_BEHAVIOR_MESSAGE { get; }

        public abstract string IS_LATEST_VERSION { get; }

        public abstract string CHECK_UPDATE_FAILD { get;}

        public abstract string NEW_VERSION_DETECTED { get; }

        public abstract string BAN_PLAYER { get; }

        public abstract string KICK_PLAYER { get; }

        public abstract string KICK_OR_BAN_REASON { get; }

        public abstract string ONLINE_PLAYER_LISTS_LOADING { get; }

        public abstract string ONLINE_PLAYER_LISTS_LOADED { get; }

        public abstract string CHANGE_LOG { get; }

        public abstract string BANNED_PLAYER_WAS_REFUSED_TO_JOIN_LOBBY { get; }

        public abstract string ONLINE_BANNED { get; }

        public abstract string LOCAL_BANNED { get; }

        public abstract string LOCAL_UNBAN_PLAYER_MESSAGE { get; }
    }
}
