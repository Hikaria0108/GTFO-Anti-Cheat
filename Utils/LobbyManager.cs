using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using Player;
using SNetwork;
using Steamworks;
using UnityEngine;
using System.Linq;
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
        }

        public bool IsPlayerBanned(SNet_Player player, CSteamID steamID)
        {
            bool flag = !LobbyManager.Host;
            if (!flag)
            {
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
                this.Blacklist.Add(item);
                this.KickPlayer(slot);
                ChatManager.AddQueue(string.Format("<#F80>作弊玩家 {0} 由于 {1} 被 {2}", player.NickName, reason, EntryPoint.Language.BAN));
                Logs.LogMessage(string.Format("{0} has been banned from the lobby for {1}", player.NickName, reason));
                List<string> list = File.ReadAllLines(this.File_Blacklist).ToList<string>();
                list.Add(item.ToString());
                File.WriteAllLines(this.File_Blacklist, list);
            }
        }

        public void UnbanPlayer(SNet_Player player, CSteamID steamID)
        {
            bool flag = this.Blacklist.Remove(steamID);
            if (flag)
            {
                List<string> list = File.ReadAllLines(this.File_Blacklist).ToList<string>();
                list.Remove(steamID.ToString());
                File.WriteAllLines(this.File_Blacklist, list);
            }
            else
            {
                Logs.LogError("Failed to remove player " + player.NickName + " from the blacklist. SteamID not found");
            }
        }

        public void WhitelistPlayer(CSteamID steamID)
        {
            bool flag = this.Whitelist.Contains(steamID);
            if (!flag)
            {
                this.Whitelist.Add(steamID);
                Logs.LogMessage("Invite sent. Player added to whitelist");
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
                ChatManager.AddQueue(string.Format("<#F80>作弊玩家 {0} 由于 {1} 被 {2}", player.NickName, reason, EntryPoint.Language.KICK));
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

        public string File_Blacklist { get; set; }

        public string File_Whitelist { get; set; }

        public List<CSteamID> Blacklist { get; set; } = new List<CSteamID>();

        public List<CSteamID> Whitelist { get; set; } = new List<CSteamID>();

        internal static LobbyManager Instance;

        private static Dictionary<string, Thread> alerts = new Dictionary<string, Thread>();

        private static Dictionary<string, Timer> alertsInstance = new Dictionary<string, Timer>();

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
                ChatManager.AddQueue(string.Format("<#F80>[GTFO Anti-Cheat] 检测到玩家 {0} 的作弊行为: {1}", _player.NickName, _reason));

                string choice;

                if (EntryPoint.AutoBanPlayer)
                {
                    _choice = KickChoise.BAN;
                    choice = EntryPoint.Language.BAN;
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
                    case KickChoise.BAN:
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
    }

    [Flags]
    public enum KickChoise
    {
        NONE = 1,
        KICK = 2,
        BAN = 3
    }
}
