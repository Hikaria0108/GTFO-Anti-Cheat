using System;
using Hikaria.GTFO_Anti_Cheat.Managers;
using Hikaria.GTFO_Anti_Cheat.Utils;

namespace Hikaria.GTFO_Anti_Cheat.Patches
{
    internal class ChatBoxCommand : Patch
    {
        public override string Name { get; } = "ChatBoxCommand";

        public static Patch Instance { get; private set; }

        public override void Initialize()
        {
            ChatBoxCommand.Instance = this;
        }

        public override void Execute()
        {
            base.PatchMethod<PlayerChatManager>("PostMessage", Patch.PatchType.Prefix, null, null, null);
        }

        private static void PlayerChatManager__PostMessage__Prefix(PlayerChatManager __instance)
        {
            string text = __instance.m_currentValue;
            try
            {
                if (text.Substring(0, 4).ToLower() == "/gac")
                {
                    text = text.Substring(1, text.Length - 1);
                    string[] array = text.Split(' ');
                    try
                    {
                        if (array[0].ToLower() == "gac")
                        {
                            string a = array[1].ToLower();
                            switch(a)
                            {
                                case "help":
                                    PrintCommands();
                                    return;
                                case "broadcast":
                                    EnableBroadcast(StringToBool(array[2]));
                                    return;
                                case "autokick":
                                    AutoKickAndBan(a, StringToBool(array[2]));
                                    return;
                                case "autoban":
                                    AutoKickAndBan(a, StringToBool(array[2]));
                                    return;
                                case "detect":
                                    Detect(array[2].ToLower(), StringToBool(array[3]));
                                    return;
                                case "unban":
                                    LobbyManager.Current.UnBanPlayer(Convert.ToUInt64(array[2]));
                                    return;
                            }
                            throw new Exception("Unknown Command");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logs.LogError(ex.Message);
                        GameEventLogManager.AddLog(EntryPoint.Language.COMMAND_UNKNOWN_HELP);
                    }
                    finally
                    {
                        __instance.m_currentValue = "";
                    }
                }
            }
            catch
            {
            }
        }


        public static bool StringToBool(string param)
        {
            param = param.ToLower();
            bool result;
            if (param == "on")
            {
                result = true;
            }
            else
            {
                if (!(param == "off"))
                {
                    throw new Exception("非法参数");
                }
                result = false;
            }
            return result;
        }

        private static void PrintCommands()
        {
            GameEventLogManager.AddLog(EntryPoint.Language.COMMAND_LIST);
            foreach (string s in commands)
            {
                GameEventLogManager.AddLog(s);
            }
        }

        private static void EnableBroadcast(bool enable)
        {
            EntryPoint.EnableBroadcast = enable;
            GameEventLogManager.AddLog (string.Format(EntryPoint.Language.COMMAND_HINT_BROADCAST, EntryPoint.EnableBroadcast ? "green" : "red", EntryPoint.EnableBroadcast ? EntryPoint.Language.TURN_ON : EntryPoint.Language.TURN_OFF));
        }

        private static void Detect(string choice, bool enable)
        {
            switch (choice)
            {
                case "booster":
                    EntryPoint.DetectBoosterHack = enable;
                    GameEventLogManager.AddLog(string.Format(EntryPoint.Language.COMMAND_HINT_DETECT_BOOSTER_HACK, EntryPoint.DetectBoosterHack ? "green" : "red", EntryPoint.DetectBoosterHack ? EntryPoint.Language.TURN_ON : EntryPoint.Language.TURN_OFF));
                    return;
                case "weapondata":
                    EntryPoint.DetectWeaponDataHack = enable;
                    GameEventLogManager.AddLog(string.Format(EntryPoint.Language.COMMAND_HINT_DETECT_WEAPON_DATA_HACK, EntryPoint.DetectWeaponDataHack ? "green" : "red", EntryPoint.DetectWeaponDataHack ? EntryPoint.Language.TURN_ON : EntryPoint.Language.TURN_OFF));
                    break;
                default:
                    throw new Exception("Unknown Command");
            }
        }

        private static void AutoKickAndBan(string choice, bool enable)
        {
            switch(choice)
            {
                case "autokick":
                    EntryPoint.AutoKickPlayer = enable;
                    GameEventLogManager.AddLog(string.Format(EntryPoint.Language.COMMAND_HINT_AUTOKICK, EntryPoint.AutoKickPlayer ? "green" : "red", EntryPoint.AutoKickPlayer ? EntryPoint.Language.TURN_ON : EntryPoint.Language.TURN_OFF));
                    break;
                case "autoban":
                    EntryPoint.AutoBanPlayer = enable;
                    GameEventLogManager.AddLog(string.Format(EntryPoint.Language.COMMAND_HINT_AUTOBAN, EntryPoint.AutoBanPlayer ? "green" : "red", EntryPoint.AutoBanPlayer ? EntryPoint.Language.TURN_ON : EntryPoint.Language.TURN_OFF));
                    break;
                default:
                    throw new Exception("Unknown Command");
            }
        }

        private static string[] commands =
        {
            EntryPoint.Language.COMMAND_DESC_BROADCAST,
            EntryPoint.Language.COMMAND_DESC_AUTO_KICK_PLAYER,
            EntryPoint.Language.COMMAND_DESC_AUTO_BAN_PLAYER,
            EntryPoint.Language.COMMAND_DESC_DETECT_BOOSTER_HACK,
            EntryPoint.Language.COMMAND_DESC_DETECT_WEAPON_DATA_HACK
        };

        private const string PatchName = "ChatBoxCommand";
    }
}
