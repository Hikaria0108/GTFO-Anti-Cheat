using System;
using System.IO;
using BepInEx;
using Player;
using SNetwork;
using Steamworks;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Hikaria.GTFO_Anti_Cheat.Utils
{
    internal class LobbyManager : ScriptableObject
    {
        private void Awake()
        {
            this.File_Blacklist = Path.Combine(Paths.ConfigPath, "Hikaria.GTFO_Anti-Cheat.blacklist.txt");
            this.File_Whitelist = Path.Combine(Paths.ConfigPath, "Hikaria.GTFO_Anti-Cheat.whitelist.txt");

            bool flag = !File.Exists(this.File_Blacklist);
            if (flag)
            {
                File.CreateText(this.File_Blacklist);
                Logs.LogInfo("Generating blacklist.txt");
            }

            bool flag2 = !File.Exists(this.File_Whitelist);
            if (flag2)
            {
                File.CreateText(this.File_Whitelist);
                Logs.LogInfo("Generating whitelist.txt");
            }

            string[] array2 = File.ReadAllLines(this.File_Whitelist);
            foreach (string value in array2)
            {
                ulong id = Convert.ToUInt64(value);
                CSteamID item = new CSteamID(id);
                Logs.LogMessage(string.Format("Whitelist: {0}", id));
                this.Whitelist.Add(item);
            }

            string[] array = File.ReadAllLines(this.File_Blacklist);
            foreach (string value in array)
            {
                ulong id = Convert.ToUInt64(value);
                CSteamID item = new CSteamID(id);
                Logs.LogMessage(string.Format("Blacklist: {0}", id));
                this.Blacklist.Add(item);
            }

            CheckConflictInTwoLists();
        }

        //从黑名单中剔除白名单玩家
        private void CheckConflictInTwoLists()
        {
            foreach (CSteamID steamID in Whitelist)
            {
                if (Blacklist.Contains(steamID))
                {
                    UnBanPlayer(steamID);
                }
            }
        }

        public bool IsOnlineBlacklistPlayer(CSteamID steamID)
        {
            return OnlineBlacklist.Contains(steamID);
        }

        public bool IsOnlineWhitelistPlayer(CSteamID steamID)
        {
            return OnlineWhitelist.Contains(steamID);
        }

        public bool IsWhitelistPlayer(CSteamID steamID)
        {
            bool flag = !LobbyManager.Host;
            if (!flag)
            {
                return this.Whitelist.Contains(steamID);
            }
            return false;
        }

        public bool IsWhitelistPlayer(SNet_Player player)
        {
            bool flag = !LobbyManager.Host;
            if (!flag)
            {
                CSteamID steamID = new CSteamID(player.Profile.player.lookup);
                return this.Whitelist.Contains(steamID);
            }
            return false;
        }

        public bool IsPlayerBanned(CSteamID steamID)
        {
            bool flag = !LobbyManager.Host;
            if (!flag)
            {
                return this.Blacklist.Contains(steamID);
            }
            return false;
        }

        public bool IsPlayerBanned(SNet_Player player)
        {
            bool flag = !LobbyManager.Host;
            if (!flag)
            {
                CSteamID steamID = new CSteamID(player.Profile.player.lookup);
                return this.Blacklist.Contains(steamID);
            }
            return false;
        }

        public void BanPlayer(int slot, string reason)
        {
            bool flag = !LobbyManager.Host;
            if (!flag)
            {
                SNet_Player player = SNet.Slots.PlayerSlots[slot].player;
                CSteamID item = new CSteamID(player.Profile.player.lookup);
                BlacklistPlayer(item);
                KickPlayer(slot);

                if (EntryPoint.EnableBroadcast)
                {
                    ChatManager.AddQueue(string.Format(EntryPoint.Language.BAN_PLAYER, player.NickName));
                    ChatManager.AddQueue(string.Format(EntryPoint.Language.KICK_OR_BAN_REASON, reason));
                }
            }
        }

        public void UnBanPlayer(SNet_Player player)
        {
            CSteamID steamID = new CSteamID(player.Profile.player.lookup);
            bool flag = this.Blacklist.Remove(steamID);
            if (flag)
            {
                System.Collections.Generic.List<string> list = File.ReadAllLines(this.File_Blacklist).ToList<string>();
                list.Remove(steamID.ToString());
                File.WriteAllLines(this.File_Blacklist, list);
                Logs.LogMessage("Remove player " + player.NickName + " from the blacklist.");
            }
            else
            {
                Logs.LogError("Failed to remove player " + player.NickName + " from the blacklist. SteamID not found!");
            }
        }

        public void UnBanPlayer(ulong id)
        {
            CSteamID steamID = new CSteamID(id);
            UnBanPlayer(steamID);
            GameEventLogManager.AddLog(string.Format(EntryPoint.Language.LOCAL_UNBAN_PLAYER_MESSAGE, id));
        }

        public void UnBanPlayer(CSteamID steamID)
        {
            bool flag = this.Blacklist.Remove(steamID);
            if (flag)
            {
                System.Collections.Generic.List<string> list = File.ReadAllLines(this.File_Blacklist).ToList<string>();
                list.Remove(steamID.ToString());
                File.WriteAllLines(this.File_Blacklist, list);
                Logs.LogMessage("Remove steamid " + steamID.ToString() + " from the blacklist.");
                return;
            }
            else
            {
                Logs.LogError("Failed to remove steamid " + steamID.ToString() + " from the blacklist. SteamID not found!");
            }
        }

        public void UnWhitePlayer(CSteamID steamID)
        {
            bool flag = this.Whitelist.Remove(steamID);
            if (flag)
            {
                System.Collections.Generic.List<string> list = File.ReadAllLines(this.File_Whitelist).ToList<string>();
                list.Remove(steamID.ToString());
                File.WriteAllLines(this.File_Whitelist, list);
                Logs.LogMessage("Remove steamid " + steamID.ToString() + " from the whitelist.");
                return;
            }
            else
            {
                Logs.LogError("Failed to remove steamid " + steamID.ToString() + " from the whitelist. SteamID not found!");
            }
        }

        public void WhitelistPlayer(CSteamID steamID, bool writeToDisk = false)
        {
            //检查是否是黑名单玩家，若是则移除
            bool flag2 = IsPlayerBanned(steamID);
            if (flag2)
            {
                UnBanPlayer(steamID);
            }

            //检查是否是白名单玩家，若不是则添加
            bool flag = this.Whitelist.Contains(steamID);
            if (!flag)
            {
                this.Whitelist.Add(steamID);
                System.Collections.Generic.List<string> list = File.ReadAllLines(this.File_Whitelist).ToList<string>();
                list.Add(steamID.ToString());
                File.WriteAllLines(this.File_Whitelist, list);
            }
        }

        public void BlacklistPlayer(CSteamID steamID, bool writeToDisk = false)
        {
            //检查是否是白名单玩家，若是则移除
            bool flag2 = IsWhitelistPlayer(steamID);
            if (flag2)
            {
               UnWhitePlayer(steamID);
            }

            //检查是否是黑名单玩家，若不是则添加
            bool flag = IsPlayerBanned(steamID);
            if (!flag)
            {
                this.Blacklist.Add(steamID);
                System.Collections.Generic.List<string> list = File.ReadAllLines(this.File_Blacklist).ToList<string>();
                list.Add(steamID.ToString());
                File.WriteAllLines(this.File_Blacklist, list);
            }
        }

        public void KickPlayer(int slot, string reason)
        {
            bool flag = !LobbyManager.Host;
            if (!flag)
            {
                SNet_Player player = SNet.Slots.PlayerSlots[slot].player;
                SNet.SessionHub.KickPlayer(player, SNet_PlayerEventReason.Kick_ByVote);
                PlayerBackpackManager.DestroyBackpack(player);

                if (EntryPoint.EnableBroadcast)
                {
                    ChatManager.AddQueue(string.Format(EntryPoint.Language.KICK_PLAYER, player.NickName));
                    ChatManager.AddQueue(string.Format(EntryPoint.Language.KICK_OR_BAN_REASON, reason));
                }
                Logs.LogMessage(string.Format("{0} has been kicked from the lobby for {1}", player.NickName, reason));
            }
        }

        public void KickPlayer(int slot)
        {
            bool flag = !LobbyManager.Host;
            if (!flag)
            {
                SNet_Player player = SNet.Slots.PlayerSlots[slot].player;
                SNet.SessionHub.KickPlayer(player, SNet_PlayerEventReason.Kick_ByVote);
                PlayerBackpackManager.DestroyBackpack(player);
                Logs.LogMessage(string.Format("{0} has been kicked from the lobby", player.NickName));
            }
        }

        public static void LoadOnlineLists()
        {
            new Task(delegate ()
            {
                GameEventLogManager.AddLog(EntryPoint.Language.ONLINE_PLAYER_LISTS_LOADING);

                string[] onlineWhitelist = HttpHelper.Get(PlayerWhitelistURL).Result;
                string[] onlineBlacklist = HttpHelper.Get(PlayerBlacklistURL).Result;

                LobbyManager.Current.OnlineWhitelist.Clear();
                foreach (string steamID in onlineWhitelist)
                {
                    if (steamID != string.Empty)
                    {
                        LobbyManager.Current.OnlineWhitelist.Add(new CSteamID(Convert.ToUInt64(steamID)));
                    }
                }

                LobbyManager.Current.OnlineBlacklist.Clear();
                foreach (string steamID in onlineBlacklist)
                {
                    if (steamID != string.Empty)
                    {
                        LobbyManager.Current.OnlineBlacklist.Add(new CSteamID(Convert.ToUInt64(steamID)));
                    }
                }               

                GameEventLogManager.AddLog(EntryPoint.Language.ONLINE_PLAYER_LISTS_LOADED);
            }).Start();
        }

        public static bool Host
        {
            get
            {
                return SNet.IsMaster;
            }
        }

        public static LobbyManager Current
        {
            get
            {
                bool flag = LobbyManager.Instance == null;
                if (flag)
                {
                    LobbyManager.Instance = ScriptableObject.CreateInstance<LobbyManager>();
                    LobbyManager.Instance.name = "Current";
                    UnityEngine.Object.DontDestroyOnLoad(LobbyManager.Instance);
                }
                return LobbyManager.Instance;
            }
        }

        private static readonly string PlayerWhitelistURL = "https://raw.githubusercontent.com/Hikaria0108/GTFO-Anti-Cheat/main/globalplayerwhitelist.txt";

        private static readonly string PlayerBlacklistURL = "https://raw.githubusercontent.com/Hikaria0108/GTFO-Anti-Cheat/main/globalplayerblacklist.txt";

        public string File_Blacklist { get; set; }

        public string File_Whitelist { get; set; }

        public System.Collections.Generic.List<CSteamID> Blacklist { get; private set; } = new System.Collections.Generic.List<CSteamID>();

        public System.Collections.Generic.List<CSteamID> Whitelist { get; private set; } = new System.Collections.Generic.List<CSteamID>();

        public System.Collections.Generic.List<CSteamID> OnlineBlacklist { get; private set; } = new System.Collections.Generic.List<CSteamID>();

        public System.Collections.Generic.List<CSteamID> OnlineWhitelist { get; private set; } = new System.Collections.Generic.List<CSteamID>();

        internal static LobbyManager Instance;

        /*
        private static System.Collections.Generic.Dictionary<string, Thread> alerts = new System.Collections.Generic.Dictionary<string, Thread>();

        private static System.Collections.Generic.Dictionary<string, Timer> alertsInstance = new System.Collections.Generic.Dictionary<string, Timer>();

        public static void StartKickorBanTimer(SNet_Player player, string reason, string threadName)
        {
            if (!alerts.ContainsKey(threadName))
            {
                CreateThread(player, reason, threadName, true);
                return;
            }
            if (alerts[threadName].IsAlive)
            {
                alertsInstance[threadName].add();
                return;
            }
            CreateThread(player, reason, threadName, false);
        }

        private static void CreateThread(SNet_Player player, string reason, string threadName, bool add)
        {
            Timer timer = new Timer(player, reason);
            Thread thread = new Thread(delegate ()
            {
                timer.Start();
            });
            thread.Start();
            if (add)
            {
                alerts.Add(threadName, thread);
                alertsInstance.Add(threadName, timer);
                return;
            }
            alerts[threadName] = thread;
            alertsInstance[threadName] = timer;
        }


        private class Timer
        {
            public void add()
            {
                this._sec = 10;
            }

            public void Start()
            {
                ChatManager.AddQueue(string.Format(EntryPoint.Language.CHEATER_DETECTED_MESSAGE, _player.NickName));
                ChatManager.AddQueue(string.Format(EntryPoint.Language.CHEATING_BEHAVIOR_MESSAGE, _reason));

                string choice;

                if (EntryPoint.AutoBanPlayer)
                {
                    _choice = KickChoise.KICKANDBAN;
                    choice = EntryPoint.Language.KICKANDBAN;
                }
                else if (EntryPoint.AutoKickPlayer)
                {
                    _choice = KickChoise.KICK;
                    choice = EntryPoint.Language.KICK;
                }
                else
                {
                    _choice = KickChoise.NONE;
                    return;
                }

                while (this._sec > 0)
                {
                    ChatManager.AddQueue(string.Format("<#F80>[GTFO Anti-Cheat] 作弊玩家 {0} 将在 {1} 秒后被 {2}", _player.NickName, _sec, choice));
                    Thread.Sleep(1000);
                    this._sec--;
                }

                switch(_choice)
                {
                    case KickChoise.KICK:
                        LobbyManager.Current.KickPlayer(_player.PlayerSlotIndex(), _reason);
                        break;
                    case KickChoise.KICKANDBAN:
                        LobbyManager.Current.BanPlayer(_player.PlayerSlotIndex(), _reason);
                        break;
                    case KickChoise.NONE:
                        return;
                }

                ChatManager.AddQueue(string.Format("[GTFO Anti-Cheat] 作弊玩家 {0} 因 {1} 已被 {2}", _player.NickName, _reason, choice));
            }

            public Timer(SNet_Player player, string reason)
            {
                _player = player;
                _reason = reason;
            }

            private int _sec = 10;

            private string _reason;

            private KickChoise _choice;

            private SNet_Player _player;
        }

        [Flags]
        public enum KickChoise
        {
            NONE = 1,
            KICK = 2,
            KICKANDBAN = 3
        }
        */
    }
}
