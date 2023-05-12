using GameData;
using Gear;
using Hikaria.GTFO_Anti_Cheat.Managers;
using Hikaria.GTFO_Anti_Cheat.Utils;
using SNetwork;

namespace Hikaria.GTFO_Anti_Cheat.Patches
{
    internal class DetectWeaponDataHack : Patch
    {
        public override void Execute()
        {
            base.PatchMethod<GameDataInit>("Initialize", PatchType.Postfix);
            base.PatchMethod<SNet_GlobalManager>("OnReplicationRoleModeChange", PatchType.Postfix);
            base.PatchMethod<Dam_EnemyDamageBase>("ReceiveBulletDamage", PatchType.Prefix);
            base.PatchMethod<Dam_EnemyDamageBase>("ReceiveMeleeDamage", PatchType.Prefix);
            
        }

        private static void GameDataInit__Initialize__Postfix()
        {
            WeaponDataManager.LoadData();
        }

        private static void SNet_GlobalManager__OnReplicationRoleModeChange__Postfix(SNet_Player player, eReplicationMode mode)
        {
            if (LobbyManager.Host && EntryPoint.DetectWeaponModelHack)
            {
                if (player == SNet.LocalPlayer || player.IsBot) //不检测自身和机器人，因为没有必要)
                {
                    return;
                }

                if (mode == eReplicationMode.ReadyToStartPlaying) //当玩家准备好开始游戏时再检测，降低资源消耗
                {
                    foreach (GearIDRange gearIDRange in GearManager.Current.m_gearPerSlot[player.PlayerSlotIndex()])
                    {
                        bool flag = WeaponDataManager.CheckIsValidWeaponGearIDRangeData(gearIDRange);
                        if (!flag)
                        {
                            LobbyManager.KickorBanPlayer(player, EntryPoint.Language.WEAPON_MODEL_HACK);
                        }
                    }
                }
            }
        }

        private static void Dam_EnemyDamageBase__ReceiveBulletDamage__Prefix(Dam_EnemyDamageBase __instance, pBulletDamageData data)
        {
            if (LobbyManager.Host && EntryPoint.DetectWeaponDataHack)
            {
                bool flag = WeaponDamageManager.CheckIsValidBulletWeaponDamage(__instance, data);
                if (!flag)
                {
                    IReplicator replicator;
                    data.source.pRep.TryGetID(out replicator);
                    SNet_Player player = replicator.OwningPlayer;

                    LobbyManager.KickorBanPlayer(player, EntryPoint.Language.WEAPON_DAMAGE_HACK);
                }
            }
        }
        private static void Dam_EnemyDamageBase__ReceiveMeleeDamage__Prefix(Dam_EnemyDamageBase __instance, pFullDamageData data)
        {
            if (LobbyManager.Host && EntryPoint.DetectWeaponDataHack)
            {
                bool flag = WeaponDamageManager.CheckIsValidMeleeDamage(__instance, data);

                if (!flag)
                {
                    IReplicator replicator;
                    data.source.pRep.TryGetID(out replicator);
                    SNet_Player player = replicator.OwningPlayer;

                    LobbyManager.KickorBanPlayer(player, EntryPoint.Language.WEAPON_DAMAGE_HACK);
                }
            }
        }



        public override string Name { get; } = "DetectWeaponDataHack";

        public static Patch Instance { get; private set; }

        public override void Initialize()
        {
            DetectWeaponDataHack.Instance = this;
        }

        private const string PatchName = "DetectWeaponDataHack";
    }
}
