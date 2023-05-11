using Hikaria.GTFO_Anti_Cheat.Managers;
using BoosterImplants;
using SNetwork;
using GameData;
using Hikaria.GTFO_Anti_Cheat.Utils;

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
            BoosterDataManager.StorePlayerBoosters(player, pBoosterImplantsWithOwner);

            if (LobbyManager.Host && EntryPoint.DetectBoosterHack)
            {
                bool flag = BoosterDataManager.CheckBoosters(pBoosterImplantsWithOwner);

                if (!flag)
                {
                    if (EntryPoint.EnableDebugInfo)
                    {
                        Logs.LogMessage(string.Format("{0} has invalid boosters", player.NickName));
                    }

                    ChatManager.DetectBroadcast(player.NickName, EntryPoint.Language.BOOSTER_HACK);

                    if (EntryPoint.AutoBanPlayer)
                    {
                        LobbyManager.Current.BanPlayer(player.PlayerSlotIndex(), EntryPoint.Language.BOOSTER_HACK);
                    }
                    else if (EntryPoint.AutoKickPlayer)
                    {
                        LobbyManager.Current.KickPlayer(player.PlayerSlotIndex(), EntryPoint.Language.BOOSTER_HACK);
                    }
                }
            }
        }

        private static void GameDataInit__Initialize__Postfix()
        {
            BoosterDataManager.AddOldValidBoosterTemplateDataBlock();
            BoosterDataManager.LoadData();
        }

        private const string PatchName = "DetectBoosterDataHack";
    }
}