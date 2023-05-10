using System;
using System.Collections.Generic;
using System.Threading;
using Hikaria.GTFO_Anti_Cheat.Utils;
using SNetwork;
using Steamworks;

namespace Hikaria.GTFO_Anti_Cheat.Patches
{
    internal class PlayerJoinLobby : Patch
    {
        public override string Name { get; } = "PlayerJoinLobby";

        public static Patch Instance { get; private set; }

        public override void Initialize()
        {
            PlayerJoinLobby.Instance = this;
        }

        public override void Execute()
        {
            base.PatchMethod<GS_Lobby>("OnPlayerEvent", PatchType.Both);
            base.PatchMethod<SNet_Lobby_STEAM>("PlayerJoined", new Type[] { typeof(SNet_Player), typeof(CSteamID) }, PatchType.Prefix);
            base.PatchMethod(typeof(SteamMatchmaking), "InviteUserToLobby", PatchType.Prefix);
            base.PatchMethod<GS_Lobby>("OnMasterChanged", PatchType.Postfix);
        }

        private static void GS_Lobby__OnPlayerEvent__Postfix(SNet_Player player, SNet_PlayerEvent playerEvent, SNet_PlayerEventReason reason)
        {
            if (playerEvent == SNet_PlayerEvent.PlayerAgentSpawned)
            {
                if (player == SNet.LocalPlayer)
                {
                    if(!EntryPoint.IsLogged)
                    {
                        EntryPoint.IsLogged = true;
                        HttpHelper.CheckUpdate();
                    }

                    if (!SNet.IsMaster)
                    {
                        GameEventLogManager.AddLog(EntryPoint.Language.NOT_HOST);
                    }
                    else
                    {
                        GameEventLogManager.AddLog(EntryPoint.Language.IS_HOST);

                        if (EntryPoint.EnableOnlinePlayerLists)
                        {
                            LobbyManager.LoadOnlineLists();
                        }
                    }
                }

                if (SNet.IsMaster)
                {
                    string threadName = "BROARDCAST";
                    if (!alerts.ContainsKey(threadName))
                    {
                        CreateThread(EntryPoint.Language.ANTI_CHEAT_BROADCAST, threadName, true);
                        return;
                    }
                    if (alerts[threadName].IsAlive)
                    {
                        alertsInstance[threadName].add();
                        return;
                    }
                    CreateThread(EntryPoint.Language.ANTI_CHEAT_BROADCAST, threadName, false);
                }
            }
        }

        private static void GS_Lobby__OnPlayerEvent__Prefix(SNet_Player player, SNet_PlayerEvent playerEvent, SNet_PlayerEventReason reason)
        {
            if (playerEvent == SNet_PlayerEvent.PlayerLeftSessionHub)
            {
                if (player == SNet.LocalPlayer)
                {
                    ChatManager.ClearQueue();
                }
            }
        }

        private static void GS_Lobby__OnMasterChanged__Postfix()
        {
            if (SNet.IsMaster)
            {
                string threadName = "BROARDCAST";
                if (!alerts.ContainsKey(threadName))
                {
                    CreateThread(EntryPoint.Language.ANTI_CHEAT_BROADCAST, threadName, true);
                    return;
                }
                if (alerts[threadName].IsAlive)
                {
                    alertsInstance[threadName].add();
                    return;
                }
                CreateThread(EntryPoint.Language.ANTI_CHEAT_BROADCAST, threadName, false);
            }
        }

        private static bool SNet_Lobby_STEAM__PlayerJoined__Prefix(SNet_Player player)
        {
            if (!LobbyManager.Host)
            {
                return true;
            }

            //若启用在线名单则优先匹配在线名单
            if (EntryPoint.EnableOnlinePlayerLists)
            {
                CSteamID steamID = new CSteamID(player.Profile.player.lookup);
                bool result1 = LobbyManager.Current.IsOnlineWhitelistPlayer(steamID) || !LobbyManager.Current.IsOnlineBlacklistPlayer(steamID);
                if (!result1)
                {
                    GameEventLogManager.AddLog(string.Format(EntryPoint.Language.BANNED_PLAYER_WAS_REFUSED_TO_JOIN_LOBBY, EntryPoint.Language.ONLINE_BANNED, player.NickName, player.Profile.player.lookup));
                    return result1;
                }
            }

            //匹配本地名单
            bool result2 = LobbyManager.Current.IsWhitelistPlayer(player) || !LobbyManager.Current.IsPlayerBanned(player);
            if (!result2)
                GameEventLogManager.AddLog(string.Format(EntryPoint.Language.BANNED_PLAYER_WAS_REFUSED_TO_JOIN_LOBBY, EntryPoint.Language.LOCAL_BANNED, player.NickName, player.Profile.player.lookup));
            return result2;
        }

        private static void SteamMatchmaking__InviteUserToLobby__Prefix(CSteamID steamIDInvitee)
        {
            LobbyManager.Current.WhitelistPlayer(steamIDInvitee);
        }

        private static void CreateThread(string msg, string threadName, bool add)
        {
            Timer timer = new Timer(msg);
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

        private const string PatchName = "PlayerJoinLobby";

        private static Dictionary<string, Thread> alerts = new Dictionary<string, Thread>();

        private static Dictionary<string, Timer> alertsInstance = new Dictionary<string, Timer>();

        private class Timer
        {
            public void add()
            {
                this._sec = 10;
            }

            public void Start()
            {
                while (this._sec > 0)
                {
                    Thread.Sleep(1000);
                    this._sec--;
                }
                if(EntryPoint.IsEnglish)
                {
                    ChatManager.SpeakInSeparate(_msg);
                }
                else
                {
                    ChatManager.AddQueue(_msg);
                }
            }

            public Timer(string msg)
            {
                _msg = msg;
            }

            private string _msg;

            private int _sec = 10;
        }
    }
}
