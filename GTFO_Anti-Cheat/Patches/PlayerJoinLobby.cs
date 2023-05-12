using System;
using System.Collections.Generic;
using System.Threading;
using Hikaria.GTFO_Anti_Cheat.Managers;
using SNetwork;
using Steamworks;
using Hikaria.GTFO_Anti_Cheat.Utils;

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
            base.PatchMethod<SNet_Lobby_STEAM>("PlayerJoined", new Type[] {
                typeof(SNet_Player),
                typeof(CSteamID) 
            }, PatchType.Prefix);
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
                        GameEventLogManager.AddLog(EntryPoint.Language.COMMAND_HELP);
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
                    AutoCreateThread(EntryPoint.Language.ANTI_CHEAT_BROADCAST, "BROADCAST_ANTI-CHEAT");
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
                AutoCreateThread(EntryPoint.Language.ANTI_CHEAT_BROADCAST, "BROADCAST_ANTI-CHEAT");

                //进行数据检测
                foreach (SNet_Player player in SNet.LobbyPlayers)
                {
                    //不检测自身和机器人，因为没有必要
                    if (player == SNet.LocalPlayer || player.IsBot) 
                    {
                        continue;
                    }

                    if (!BoosterDataManager.CheckBoostersForPlayer(player))
                    {
                        LobbyManager.KickorBanPlayer(player, EntryPoint.Language.BOOSTER_HACK);
                    }

                    if (!WeaponDataManager.CheckIsValidWeaponGearIDRangeDataForPlayer(player)) 
                    {
                        LobbyManager.KickorBanPlayer(player, EntryPoint.Language.WEAPON_MODEL_HACK);
                    }
                }
            }
        }

        private static bool SNet_Lobby_STEAM__PlayerJoined__Prefix(SNet_Player player)
        {
            if (!LobbyManager.Host)
            {
                return true;
            }

            return LobbyManager.CanPlayerJoinLobby(player);
        }

        private static void SteamMatchmaking__InviteUserToLobby__Prefix(CSteamID steamIDInvitee)
        {
            LobbyManager.Current.WhitelistPlayer(steamIDInvitee);
        }

        private static void AutoCreateThread(string msg, string threadName)
        {
            if (!alerts.ContainsKey(threadName))
            {
                CreateThread(msg, threadName, true);
                return;
            }
            if (alerts[threadName].IsAlive)
            {
                alertsInstance[threadName].add();
                return;
            }
            CreateThread(msg, threadName, false);
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
                this._sec = 15;
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

            private int _sec = 15;
        }
    }
}
