using Agents;
using Gear;
using Hikaria.GTFO_Anti_Cheat.Utils;
using Player;
using SNetwork;
using System;
using System.Collections;
using UnityEngine;
using static TenChambers.Backend;

namespace Hikaria.GTFO_Anti_Cheat.Managers
{
    internal class WeaponDamageManager
    {
        public static bool CheckIsValidMeleeDamage(Dam_EnemyDamageBase dam_EnemyDamageBase, pFullDamageData data)
        {
            IReplicator replicator;
            data.source.pRep.TryGetID(out replicator);
            SNet_Player player = replicator.OwningPlayer;
            int playerSlotIndex = player.PlayerSlotIndex();
            PlayerAgent playerAgent;
            PlayerManager.TryGetPlayerAgent(ref playerSlotIndex, out playerAgent);

            InventorySlot wieldSlot = playerAgent.Inventory.WieldedSlot;

            if (wieldSlot == InventorySlot.None || wieldSlot != InventorySlot.GearMelee) 
            {
#if _DEBUG
                Logs.LogMessage("MeleeDamage but not wield melee!");
#endif
                return false;
            }

            ItemEquippable wieldItem = playerAgent.Inventory.WieldedItem;

            if (wieldItem == null || wieldItem.MeleeArchetypeData == null)
            {
#if _DEBUG
                Logs.LogMessage("MeleeArchtypeData is null!");
#endif
                return false;
            }

            //获取data中的真实数据
            Vector3 direction = data.direction.Value; //子弹射击方向
            Vector3 localPosition = data.localPosition.Get(10f); //玩家到敌人的相对位置
            Vector3 position = localPosition + playerAgent.Position; //敌人位置
            float damage = data.damage.Get(dam_EnemyDamageBase.HealthMax); //需要校验的伤害数值
            float distance = data.localPosition.Get(10f).magnitude; //敌人与玩家之间的距离
            float precisionMulti = data.precisionMulti.Get(10f); //枪械精准倍率
            float staggerMulti = data.staggerMulti.Get(10f); //硬直倍率
            float backstabberMulti = data.backstabberMulti.Get(10f); //背伤倍率
            float sleeperMulti = data.sleeperMulti.Get(10f); //沉睡倍率
            int limbID = (int)data.limbID; //命中部位ID

            //获取本地数据
            //float targetDamage = 

            //float num = this.ApplyWeakspotAndArmorModifiers(dam, precisionMulti);
            //num = this.ApplyDamageFromBehindBonus(num, position, direction, backstabberMulti);


            return true;
        }

        public static bool CheckIsValidBulletWeaponDamage(Dam_EnemyDamageBase dam_EnemyDamageBase, pBulletDamageData data)
        {
#if _DEBUG
            Logs.LogMessage("=========================");
            Logs.LogMessage("Enter Check BulletDamage");
#endif

            IReplicator replicator;
            data.source.pRep.TryGetID(out replicator);
            SNet_Player player = replicator.OwningPlayer;
            int playerSlotIndex = player.PlayerSlotIndex();
            PlayerAgent playerAgent;
            PlayerManager.TryGetPlayerAgent(ref playerSlotIndex, out playerAgent);

            InventorySlot wieldSlot = playerAgent.Inventory.WieldedSlot;

            if (wieldSlot == InventorySlot.None || wieldSlot != InventorySlot.GearStandard && wieldSlot != InventorySlot.GearSpecial)
            {
#if _DEBUG
                Logs.LogMessage("BulletDamage but not wield gun!");
#endif
                return false;
            }

            ItemEquippable wieldItem = playerAgent.Inventory.WieldedItem;

            //判断是否存在ArchtypeData，不存在说明一定是魔改枪
            if (wieldItem.ArchetypeData == null)
            {
#if _DEBUG
                Logs.LogMessage("ArchtypeData is null!");
#endif
                return false;
            }

            //获取data中的真实数据
            Vector3 direction = data.direction.Value; //子弹射击方向
            Vector3 localPosition = data.localPosition.Get(10f); //玩家到敌人的相对位置
            Vector3 position = localPosition + dam_EnemyDamageBase.Owner.Position; //命中位置
            float damage = data.damage.Get(dam_EnemyDamageBase.HealthMax); //需要校验的伤害数值
            float precisionMulti = data.precisionMulti.Get(10f); //枪械精准倍率
            float staggerMulti = data.staggerMulti.Get(10f); //硬直倍率
            bool allowDirectionalBonus = data.allowDirectionalBonus; //是否允许后背加成
            int limbID = (int)data.limbID; //命中部位ID

#if _DEBUG
            Logs.LogMessage(string.Format("Data from player: damage:{0}, LimbID:{1}, precisionMulti:{2}, staggerMulti:{3}, allowDirectionBonus:{4}, localPostion:{5}", damage, limbID, precisionMulti, staggerMulti, allowDirectionalBonus, localPosition.ToDetailedString()));
#endif

            //本地数据获取
            float local_precisionMulti = wieldItem.ArchetypeData.PrecisionDamageMulti; //本地枪械精准伤害倍率
            float local_staggerMulti = wieldItem.ArchetypeData.StaggerDamageMulti; //本地枪械硬直倍率


            //判断精准倍率是否相同，在误差范围内忽略
            bool flag = Math.Abs(local_precisionMulti - precisionMulti) > 0.01f;

#if _DEBUG
            Logs.LogMessage(string.Format("precisionMulti: FromPlayer: {0}, Local: {1}, Match: {2}", precisionMulti, local_precisionMulti, !flag));
#endif
            if (flag)
            {
#if _DEBUG
                Logs.LogMessage("Exit Check BulletDamage");
                Logs.LogMessage("=========================");
#endif
                return CheckTolerance(!flag, playerSlotIndex);
            }

            bool flag2 = Math.Abs(local_staggerMulti - staggerMulti) > 0.01f;
            if (flag2)
            {
#if _DEBUG
                Logs.LogMessage(string.Format("staggerMulti: FromPlayer: {0}, Local: {1}, Match: {2}", staggerMulti, local_precisionMulti, !flag2));
                Logs.LogMessage("Exit Check BulletDamage");
                Logs.LogMessage("=========================");
#endif
                return CheckTolerance(!flag2, playerSlotIndex); ;

            }

            Vector2 falloff = wieldItem.ArchetypeData.DamageFalloff; //本地枪械伤害距离衰减

            Dam_EnemyDamageLimb limb = dam_EnemyDamageBase.DamageLimbs[limbID]; //获取击中部位，limbID是根据Index来的，不同部位的ID在不同敌人上会不同

            float distance = Vector3.Distance(position, playerAgent.Position); //计算本地距离，会因为延迟导致不精确，建议100ms以内

#if _DEBUG
            Logs.LogMessage(string.Format("playerAgent Pos: {0}, DamageTargetPos: {1}, HitPos: {2}", playerAgent.Position.ToDetailedString(), limb.DamageTargetPos, position.ToDetailedString()));
#endif

            //获取带Booster加成效果的伤害数值
            float targetDamage = wieldItem.ArchetypeData.GetDamageWithBoosterEffect(playerAgent, wieldSlot);

            //计算距离伤害衰减
            float falloffMulti = 1f;
            if (distance > falloff.x)
            {
                falloffMulti = Mathf.Max(1f - (distance - falloff.x) / (falloff.y - falloff.x), BulletWeapon.s_falloffMin);
            }

#if _DEBUG
            Logs.LogMessage(string.Format("Falloff: {0}", falloffMulti));
#endif

            targetDamage *= falloffMulti; //算上衰减倍率
            targetDamage = limb.ApplyWeakspotAndArmorModifiers(targetDamage, local_precisionMulti); //算上特殊部位伤害倍率
            targetDamage = limb.ApplyDamageFromBehindBonus(targetDamage, position, direction, 1f); //子弹伤害不算硬直倍率永远是1f

            bool flag3 = Math.Abs(damage - targetDamage) / targetDamage > 0.075f; //伤害差值比率在7.5%误差范围内认为没问题，误差原因是延迟无法避免
            
            //targetDamage = Math.Min(dam_EnemyDamageBase.HealthMax, targetDamage); //限制最大值为敌人生命值

#if _DEBUG
            Logs.LogMessage(string.Format("Damage: From player: {0}, LocalCalc: {1}, Match: {2}", damage, targetDamage, !flag3));
            Logs.LogMessage("Exit Check BulletDamage");
            Logs.LogMessage("=========================");
#endif

            return CheckTolerance(!flag3, playerSlotIndex);
        }

        private static int[] tolerance = new int[4] { 0, 0, 0, 0 };

        private static bool CheckTolerance(bool flag, int slot)
        {
            if (flag)
            {
                tolerance[slot] = 0;
#if _DEBUG
                Logs.LogMessage("Check tolerance: " + tolerance);
#endif
                return true;
            }
#if _DEBUG
            Logs.LogMessage("Check tolerance: " + tolerance);
#endif

            tolerance[slot]++;

            if (tolerance[slot] >= 5)
            {
                tolerance[slot] = 0;
#if _DEBUG
                Logs.LogMessage("Out of tolerance, reset!");
#endif
                return false;
            }

            return true;
        }
    }
}
