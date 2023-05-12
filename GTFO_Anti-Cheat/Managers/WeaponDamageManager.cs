using Hikaria.GTFO_Anti_Cheat.Utils;
using Player;
using SNetwork;
using System;
using UnityEngine;

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

            if (playerSlotIndex == -1)
            {
                return true;
            }

            PlayerAgent playerAgent;
            PlayerManager.TryGetPlayerAgent(ref playerSlotIndex, out playerAgent);

            InventorySlot wieldSlot = playerAgent.Inventory.WieldedSlot;

            if (wieldSlot != InventorySlot.GearMelee) 
            {
                if (EntryPoint.EnableDebugInfo)
                {
                    Logs.LogMessage("MeleeDamage but not melee!");
                }
                return false;
            }

            ItemEquippable item = playerAgent.Inventory.WieldedItem;

            if (item.MeleeArchetypeData == null)
            {
                if (EntryPoint.EnableDebugInfo)
                {
                    Logs.LogMessage("MeleeArchtypeData is null!");
                }
                return false;
            }

            //算法写在这
            

            return true;
        }

        public static bool CheckIsValidBulletWeaponDamage(Dam_EnemyDamageBase dam_EnemyDamageBase, pBulletDamageData data)
        {
            IReplicator replicator;
            data.source.pRep.TryGetID(out replicator);
            SNet_Player player = replicator.OwningPlayer;
            int playerSlotIndex = player.PlayerSlotIndex();

            if (player == SNet.LocalPlayer || player.IsBot) //不检测自身和机器人，因为没有必要
            {
                return true;
            }

            PlayerAgent playerAgent;
            PlayerManager.TryGetPlayerAgent(ref playerSlotIndex, out playerAgent);
            InventorySlot wieldSlot = playerAgent.Inventory.WieldedSlot;

            ItemEquippable item = playerAgent.Inventory.WieldedItem;
            
            //判断是否存在ArchtypeData，不存在说明一定是魔改枪
            if (item.ArchetypeData == null)
            {
                if (EntryPoint.EnableDebugInfo)
                {
                    Logs.LogMessage("ArchtypeData Null!");
                }
                return false;
            }

            float precisionMulti = item.ArchetypeData.PrecisionDamageMulti; //本地枪械精准伤害倍率

            float dataprecisionMulti = data.precisionMulti.Get(10f);

            bool flag = Math.Abs(precisionMulti - dataprecisionMulti) <= 0.01f;

            //判断精准倍率是否相同，在误差范围内认为相同
            if (!flag)
            {
                if (EntryPoint.EnableDebugInfo)
                {
                    Logs.LogMessage(string.Format("precisionMulti Not Match! fromPlayer:{0},local:{1}", dataprecisionMulti, precisionMulti));
                }
                return false;
            }

            //强化剂对武器的伤害加成
            float boosterWeaponDamageMulti = BoosterDataManager.GetBoosterDamageMultiForPlayer(wieldSlot, player);

            float weaponDamage = item.ArchetypeData.Damage; //本地枪械伤害数据

            Vector2 falloff = item.ArchetypeData.DamageFalloff; //本地枪械伤害距离衰减

            float distance = Vector3.Distance(dam_EnemyDamageBase.DamageTargetPos, data.localPosition.Get(10f)); //暂时还不知道怎么使用localPosion

            if (EntryPoint.EnableDebugInfo)
            {
                Logs.LogMessage(string.Format("Data from player: damage:{0}, LimbID:{1}, precisionMulti:{2}, staggerMulti:{3}, allowDirectionBonus:{4}, localPostion:{5}", data.damage.Get(dam_EnemyDamageBase.HealthMax), data.limbID, data.precisionMulti.Get(10f), data.staggerMulti.Get(10f), data.allowDirectionalBonus, data.localPosition.Get(10f).ToDetailedString()));
            }

            Dam_EnemyDamageLimb limb = dam_EnemyDamageBase.DamageLimbs[data.limbID]; //获取击中部位，LimbID是根据Index来的，会改变

            Vector3 playerPos = playerAgent.AimTarget.position;
            Vector3 enemyLimbPos = limb.DamageTargetPos;
            Vector3 fireDirection = enemyLimbPos - playerPos; //获取弹道方向，先用Aimtarget替代localPostion

            if (EntryPoint.EnableDebugInfo)
            {
                Logs.LogMessage("Test FireDirection: " + fireDirection.ToDetailedString());
            }

            distance = fireDirection.magnitude;//射击距离

            if (EntryPoint.EnableDebugInfo)
            {
                Logs.LogMessage("Distance: " + distance);
            }

            fireDirection.Normalize();//射击方向归一化

            float backBonusMulti = data.allowDirectionalBonus ? GetBackBonusMulti(dam_EnemyDamageBase, fireDirection) : 1f; //获取后背加成倍率

            float damage = data.damage.Get(dam_EnemyDamageBase.HealthMax); //接收到的伤害数据

            float falloffMulti = 1f;

            //超出伤害最大距离
            if (distance >= falloff.y)
            {
                if (damage != 0f)
                {
                    Logs.LogMessage(string.Format("over falloff but not 0 damage: {0}", damage));
                    return false;
                }
            }
            else if (distance > falloff.x) //有距离衰减
            {
                falloffMulti = 1f - (distance - falloff.x) / (falloff.y - falloff.x); //衰减算法
            }

            if (EntryPoint.EnableDebugInfo)
            {
                Logs.LogMessage("FalloffMulti: " + falloffMulti);
            }

            float targetDamage = limb.TestDamageModifiers(weaponDamage, precisionMulti) * falloffMulti * backBonusMulti * boosterWeaponDamageMulti; //伤害计算公式
            targetDamage = Math.Min(dam_EnemyDamageBase.HealthMax, targetDamage); //限制最大值为敌人生命值

            if (EntryPoint.EnableDebugInfo)
            {
                Logs.LogMessage(string.Format("damage:{0},targetDamage:{1}", damage, targetDamage));
            }

            bool flag2 = Math.Abs(damage - targetDamage) / targetDamage <= 0.025f; //在一定误差范围内认为没问题，目前由于算法缺陷问题导致精度没有很高
            if (!flag2)
            {
                //检测到改伤作弊
                return false;
            }
            return true;
        }

        public static float GetBackBonusMulti(Dam_EnemyDamageBase dam_EnemyDamageBase, Vector3 direction)
        {
            if(!dam_EnemyDamageBase.Owner.EnemyBalancingData.AllowDamgeBonusFromBehind) //首先判断敌人吃不吃背伤
            {
                return 1f;
            }

            Vector3 enemyForward = dam_EnemyDamageBase.Owner.Forward; //敌人的朝向

            //去除弹道方向的y轴分量
            direction.y = 0f; 

            float angle = Vector3.Angle(enemyForward, direction); //计算角度

            //大于90度不吃背伤，如果有realbackbonus以后再考虑，背伤倍率是固定2倍
            return angle > 90f ? 1f : 2f;
        }
    }
}
