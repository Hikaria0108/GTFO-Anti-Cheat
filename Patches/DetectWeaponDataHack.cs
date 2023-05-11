using GameData;
using Gear;
using Hikaria.GTFO_Anti_Cheat.Managers;
using Hikaria.GTFO_Anti_Cheat.Utils;
using Player;
using SNetwork;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static PlayfabMatchmakingManager.MatchResult;

namespace Hikaria.GTFO_Anti_Cheat.Patches
{
    internal class DetectWeaponDataHack : Patch
    {
        public override void Execute()
        {
            /*base.PatchMethod<PlayerBackpackManager>("EquipSyncGear",
                new Type[]
                {
                    typeof(InventorySlot),
                    typeof(GearIDRange),
                    typeof(PlayerBackpack)
                }, PatchType.Postfix);*/
            base.PatchMethod<GameDataInit>("Initialize", PatchType.Postfix);
            base.PatchMethod<SNet_GlobalManager>("OnReplicationRoleModeChange", PatchType.Postfix);
            base.PatchMethod<Dam_EnemyDamageBase>("ReceiveBulletDamage", Patch.PatchType.Prefix, null, null, null);
            //base.PatchMethod<Dam_EnemyDamageBase>("ReceiveMeleeDamage", Patch.PatchType.Prefix, null, null, null);
            
        }

        private static void GameDataInit__Initialize__Postfix()
        {
            WeaponDataManager.LoadData();
        }

        private static void PlayerBackpackManager__EquipSyncGear__Postfix(GearIDRange gearSetup, PlayerBackpack backpack)
        {
            if (LobbyManager.Host && EntryPoint.DetectWeaponModelHack)
            {
                bool flag = WeaponDataManager.CheckIsValidWeaponGearIDRangeData(gearSetup);
                if (!flag)
                {
                    ChatManager.DetectBroadcast(backpack.Owner.NickName, EntryPoint.Language.WEAPON_MODEL_HACK);

                    if (EntryPoint.AutoBanPlayer)
                    {
                        LobbyManager.Current.BanPlayer(backpack.Owner.PlayerSlotIndex(), EntryPoint.Language.WEAPON_MODEL_HACK);
                    }
                    else if (EntryPoint.AutoKickPlayer)
                    {
                        LobbyManager.Current.KickPlayer(backpack.Owner.PlayerSlotIndex(), EntryPoint.Language.WEAPON_MODEL_HACK);
                    }
                }
            }
        }
        private static void SNet_GlobalManager__OnReplicationRoleModeChange__Postfix(SNet_Player player, eReplicationMode mode)
        {
            if (LobbyManager.Host && EntryPoint.DetectWeaponModelHack)
            {
                //不检测自身，因为无法检测
                if (player == SNet.LocalPlayer)
                {
                    return;
                }

                if (mode == eReplicationMode.ReadyToStartPlaying) //当玩家在大厅时准备好开始游戏时再检测，降低资源消耗
                {
                    foreach (GearIDRange gearIDRange in GearManager.Current.m_gearPerSlot[player.PlayerSlotIndex()])
                    {
                        bool flag = WeaponDataManager.CheckIsValidWeaponGearIDRangeData(gearIDRange);
                        if (!flag)
                        {
                            ChatManager.DetectBroadcast(player.NickName, EntryPoint.Language.WEAPON_MODEL_HACK);

                            if (EntryPoint.AutoBanPlayer)
                            {
                                LobbyManager.Current.BanPlayer(player.PlayerSlotIndex(), EntryPoint.Language.WEAPON_MODEL_HACK);
                            }
                            else if (EntryPoint.AutoKickPlayer)
                            {
                                LobbyManager.Current.KickPlayer(player.PlayerSlotIndex(), EntryPoint.Language.WEAPON_MODEL_HACK);
                            }
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

                    ChatManager.DetectBroadcast(player.NickName, EntryPoint.Language.WEAPON_DAMAGE_HACK);

                    if (EntryPoint.AutoBanPlayer)
                    {
                        LobbyManager.Current.BanPlayer(player.PlayerSlotIndex(), EntryPoint.Language.WEAPON_DAMAGE_HACK);
                    }
                    else if (EntryPoint.AutoKickPlayer)
                    {
                        LobbyManager.Current.KickPlayer(player.PlayerSlotIndex(), EntryPoint.Language.WEAPON_DAMAGE_HACK);
                    }
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

                    ChatManager.DetectBroadcast(player.NickName, EntryPoint.Language.WEAPON_DAMAGE_HACK);

                    if (EntryPoint.AutoBanPlayer)
                    {
                        LobbyManager.Current.BanPlayer(player.PlayerSlotIndex(), EntryPoint.Language.WEAPON_DAMAGE_HACK);
                    }
                    else if (EntryPoint.AutoKickPlayer)
                    {
                        LobbyManager.Current.KickPlayer(player.PlayerSlotIndex(), EntryPoint.Language.WEAPON_DAMAGE_HACK);
                    }
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
