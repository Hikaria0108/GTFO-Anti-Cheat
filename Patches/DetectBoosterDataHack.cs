using Hikaria.GTFO_Anti_Cheat.Utils;
using BoosterImplants;
using SNetwork;
using GameData;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Player;
using static PlayfabMatchmakingManager.MatchResult;
using static SNetwork.SNetStructs;

namespace Hikaria.GTFO_Anti_Cheat.Patches
{
    internal class DetectBoosterDataHack : Patch
    {
        public override void Execute()
        {
            base.PatchMethod<BoosterImplantManager>("OnSyncBoosterImplants", PatchType.Postfix);
            base.PatchMethod<GameDataInit>("Initialize", PatchType.Postfix);
        }

        public override string Name { get; } = "DetectBoosterDataHack";

        public static Patch Instance { get; private set; }

        public override void Initialize()
        {
            DetectBoosterDataHack.Instance = this;
        }

        private static void BoosterImplantManager__OnSyncBoosterImplants__Postfix(SNet_Player player, pBoosterImplantsWithOwner pBoosterImplantsWithOwner)
        {
            bool flag = BoosterTemplateDataManager.CheckBoosters(pBoosterImplantsWithOwner);
            if (!flag)
            {
                if (EntryPoint.EnableDebugInfo)
                {
                    Logs.LogMessage(string.Format("{0} has invalid boosters", player.NickName));
                }

                ChatManager.Speak(string.Format(EntryPoint.Language.CHEATER_DETECTED_MESSAGE, player.NickName));
                ChatManager.Speak(string.Format(EntryPoint.Language.CHEATING_BEHAVIOR_MESSAGE, EntryPoint.Language.BOOSTER_HACK));

                if (LobbyManager.Host)
                {
                    if (EntryPoint.AutoBanPlayer)
                    {
                        LobbyManager.Current.BanPlayer(player.PlayerSlotIndex(), EntryPoint.Language.BOOSTER_HACK);
                    }
                    else if (EntryPoint.AutoKickPlayer)
                    {
                        LobbyManager.Current.KickPlayer(player.PlayerSlotIndex(), EntryPoint.Language.BOOSTER_HACK);
                    }
                    //LobbyManager.StartKickorBanTimer(player, EntryPoint.Language.BOOSTER_HACK, player.Lookup + EntryPoint.Language.BOOSTER_HACK);
                }  
            }
        }

        private static void GameDataInit__Initialize__Postfix()
        {
            BoosterTemplateDataManager.AddOldValidBoosterTemplateDataBlock();
            BoosterTemplateDataManager.LoadData();
        }

        private const string PatchName = "DetectBoosterDataHack";

        private static void StartTimer(SNet_Player player)
        {
            string threadName = player.Lookup.ToString();
            if (!alerts.ContainsKey(threadName))
            {
                CreateThread(player, threadName, true);
                return;
            }
            if (alerts[threadName].IsAlive)
            {
                alertsInstance[threadName].add();
                return;
            }
            CreateThread(player, threadName, false);
        }

        private static void CreateThread(SNet_Player player, string threadName, bool add)
        {
            Timer timer = new Timer(player);
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
                ChatManager.Speak(string.Format("<color=orange>[GTFO Anti-Cheat]</color> 检测到作弊玩家 {0}", _player.NickName));
                ChatManager.Speak(string.Format("<color=orange>[GTFO Anti-Cheat]</color> 作弊行为: {0}", EntryPoint.Language.BOOSTER_HACK));
            }

            public Timer(SNet_Player player)
            {
                _player = player;
            }

            private SNet_Player _player;

            private int _sec = 10;
        }
    }
}