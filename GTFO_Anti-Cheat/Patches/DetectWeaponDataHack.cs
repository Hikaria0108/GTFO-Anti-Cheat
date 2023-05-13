using Agents;
using GameData;
using Gear;
using Hikaria.GTFO_Anti_Cheat.Managers;
using Hikaria.GTFO_Anti_Cheat.Utils;
using Il2CppSystem.Collections.Generic;
using Player;
using SNetwork;
using System;
using UnityEngine;

namespace Hikaria.GTFO_Anti_Cheat.Patches
{
    internal class DetectWeaponDataHack : Patch
    {
        public override void Execute()
        {
            base.PatchMethod<GameDataInit>("Initialize", PatchType.Postfix);
            base.PatchMethod<PlayerBackpackManager>("ReceiveInventorySync", PatchType.Postfix);
            base.PatchMethod<Dam_EnemyDamageBase>("ReceiveBulletDamage", PatchType.Prefix);
            //base.PatchMethod<Dam_EnemyDamageBase>("ReceiveMeleeDamage", PatchType.Prefix);
            base.PatchMethod<SentryGunInstance_Firing_Bullets>("FireBullet", PatchType.Both);
        }

        private static void GameDataInit__Initialize__Postfix()
        {
            WeaponDataManager.LoadData();
        }

        private static void PlayerBackpackManager__ReceiveInventorySync__Postfix(pInventorySync data)
        {
            if (LobbyManager.Host && EntryPoint.DetectWeaponDataHack)
            {
                SNet_Player player;
                if (!data.sourcePlayer.TryGetPlayer(out player))
                {
                    return;
                }

                int playerSlotIndex = player.PlayerSlotIndex();

                if (playerSlotIndex == -1)
                {
                    return;
                }

                if (player == SNet.LocalPlayer || player.IsBot) //不检测自身和机器人，因为没有必要)
                {
                    return;
                }

                List<GearIDRange> gears = new List<GearIDRange>();
                gears.Add(new GearIDRange(data.gearStandard));
                gears.Add(new GearIDRange(data.gearSpecial));
                gears.Add(new GearIDRange(data.gearMelee));
                gears.Add(new GearIDRange(data.gearClass));
                
                foreach (GearIDRange gearIDRange in gears)
                {
                    bool flag = WeaponDataManager.CheckIsValidWeaponGearIDRangeData(gearIDRange);
                    if (!flag)
                    {
                        LobbyManager.KickorBanPlayer(player, EntryPoint.Language.WEAPON_MODEL_HACK);
                    }
                }
            }
        }

        private static void Dam_EnemyDamageBase__ReceiveBulletDamage__Prefix(Dam_EnemyDamageBase __instance, pBulletDamageData data)
        {
            if (LobbyManager.Host && EntryPoint.DetectWeaponDataHack)
            {
                if (InSentryGunFiringPendding) //排除SentryGun
                {
                    return;
                }

                IReplicator replicator;
                data.source.pRep.TryGetID(out replicator);
                SNet_Player player = replicator.OwningPlayer;

                if (player == SNet.LocalPlayer || player.IsBot) //不检测自身和机器人，因为没有必要
                {
                    return;
                }

                bool flag = WeaponDamageManager.CheckIsValidBulletWeaponDamage(__instance, data);
                if (!flag)
                {
                    LobbyManager.KickorBanPlayer(player, EntryPoint.Language.WEAPON_DATA_HACK);
                }
            }
        }

        private static void Dam_EnemyDamageBase__ReceiveMeleeDamage__Prefix(Dam_EnemyDamageBase __instance, pFullDamageData data)
        {
            if (LobbyManager.Host && EntryPoint.DetectWeaponDataHack)
            {
                IReplicator replicator;
                data.source.pRep.TryGetID(out replicator);
                SNet_Player player = replicator.OwningPlayer;

                if (player == SNet.LocalPlayer || player.IsBot) //不检测自身和机器人，因为没有必要
                {
                    return;
                }

                bool flag = WeaponDamageManager.CheckIsValidMeleeDamage(__instance, data);

                if (!flag)
                {
                    LobbyManager.KickorBanPlayer(player, EntryPoint.Language.WEAPON_DATA_HACK);
                }

            }
        }

        private static void SentryGunInstance_Firing_Bullets__FireBullet__Prefix()
        {
            InSentryGunFiringPendding = true;
        }

        private static void SentryGunInstance_Firing_Bullets__FireBullet__Postix()
        {
            InSentryGunFiringPendding = false;
        }

        private static bool InSentryGunFiringPendding;

        public override string Name { get; } = "DetectWeaponDataHack";

        public static Patch Instance { get; private set; }

        public override void Initialize()
        {
            DetectWeaponDataHack.Instance = this;
        }

        private const string PatchName = "DetectWeaponDataHack";
    }
}
