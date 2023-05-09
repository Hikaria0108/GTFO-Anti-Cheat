namespace Hikaria.GTFO_Anti_Cheat.Lang
{
    public abstract class LanguageBase
    {
        public abstract string WORKING_MESSAGE { get; }

        public abstract string COMMAND_LIST { get; }

        public abstract string CURRENT_VERSION { get; }

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

        public abstract string KICK { get; }

        public abstract string BAN { get; }

        public abstract string CHEATER_DETECTED_MESSAGE { get;}

        public abstract string CHEATING_BEHAVIOR_MESSAGE { get; }
    }
}
