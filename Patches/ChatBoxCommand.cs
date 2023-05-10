using System;
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
                                    AutoKickAndBan("autokick", StringToBool(array[2]));
                                    return;
                                case "autoban":
                                    AutoKickAndBan("autoban", StringToBool(array[2]));
                                    return;
                                case "detect":
                                    DetectBooster(array[2].ToLower(), StringToBool(array[3]));
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
                        GameEventLogManager.AddLog("<color=red>[GTFO Anti-Cheat]</color> 输入有误，输入/gac help查看帮助");
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
            GameEventLogManager.AddLog("<color=orange>[GTFO Anti-Cheat]</color> 可用命令如下:");
            foreach (string s in commands)
            {
                GameEventLogManager.AddLog(s);
            }
        }

        private static void EnableBroadcast(bool enable)
        {
            EntryPoint.EnableBroadcast = enable;
            GameEventLogManager.AddLog(string.Format("<#F80>[GTFO Anti-Cheat] <color={0}>作弊玩家通告消息已{1}</color>", EntryPoint.EnableBroadcast ? "green" : "red", EntryPoint.EnableBroadcast ? "启用" : "禁用"));
        }

        private static void DetectBooster(string choice, bool enable)
        {
            switch (choice)
            {
                case "booster":
                    EntryPoint.DetectBoosterHack = enable;
                    GameEventLogManager.AddLog(string.Format("<#F80>[GTFO Anti-Cheat] <color={0}>强化剂作弊检测已{1}</color>", EntryPoint.DetectBoosterHack ? "green" : "red", EntryPoint.DetectBoosterHack ? "启用" : "禁用"));
                    return;
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
                    GameEventLogManager.AddLog(string.Format("<#F80>[GTFO Anti-Cheat] <color={0}>自动踢出作弊玩家已{1}</color>", EntryPoint.AutoKickPlayer ? "green" : "red", EntryPoint.AutoKickPlayer ? "启用" : "禁用"));
                    break;
                case "autoban":
                    EntryPoint.AutoBanPlayer = enable;
                    GameEventLogManager.AddLog(string.Format("<#F80>[GTFO Anti-Cheat] <color={0}>自动踢出并封禁作弊玩家已{1}</color>", EntryPoint.AutoBanPlayer ? "green" : "red", EntryPoint.AutoBanPlayer ? "启用" : "禁用"));
                    break;
            }
        }

        private static string[] commands =
        {
            "/gac broadcast [on|off] 开启|关闭 作弊玩家信息通报",
            "/gac autokick [on|off], 开启|关闭 自动踢出作弊玩家",
            "/gac autoban [on|off], 开启|关闭 自动踢出并封禁作弊玩家",
            "/gac detect booster [on|off], 开启|关闭 强化剂作弊检测"
        };

        private const string PatchName = "ChatBoxCommand";
    }
}
