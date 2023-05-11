using Hikaria.GTFO_Anti_Cheat.Utils;
using Player;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hikaria.GTFO_Anti_Cheat.Managers
{
    internal class WeaponDamageManager
    {
        public static bool CheckIsValidMeleeDamage(Dam_EnemyDamageBase dam_EnemyDamageBase, pFullDamageData data)
        {
            return true;
        }

        public static bool CheckIsValidBulletWeaponDamage(Dam_EnemyDamageBase dam_EnemyDamageBase, pBulletDamageData data)
        {
            IReplicator replicator;
            data.source.pRep.TryGetID(out replicator);
            SNet_Player player = replicator.OwningPlayer;
            int playerSlotIndex = player.PlayerSlotIndex();

            if (player == SNet.LocalPlayer) //不检测自身，因为无法检测
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

            //判断精准倍率是否相同
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

            float distance = Vector3.Distance(dam_EnemyDamageBase.DamageTargetPos, data.localPosition.Get(1f)); //暂时还不知道怎么使用localPosion
            if (EntryPoint.EnableDebugInfo)
            {
                Logs.LogMessage(string.Format("damage:{0},LimbID:{1},precisionMulti:{2},staggerMulti:{3},allowDirectionBonus:{4},distance:{5}", data.damage.internalValue, data.limbID, data.precisionMulti.internalValue, data.staggerMulti.internalValue, data.allowDirectionalBonus, distance));
            }

            Dam_EnemyDamageLimb limb = dam_EnemyDamageBase.DamageLimbs[0];
            foreach (Dam_EnemyDamageLimb _limb in dam_EnemyDamageBase.DamageLimbs)
            {
                if (_limb.m_limbID == data.limbID)
                {
                    //特殊部位标记
                    limb = _limb;
                    break;
                }
            }

            Vector3 playerPos = playerAgent.AimTarget.position;
            Vector3 enemyLimbPos = limb.DamageTargetPos;
            Vector3 direction = enemyLimbPos - playerPos; //获取弹道方向，先用Aimtarget替代localPostion

            distance = direction.magnitude;//射击距离

            if (EntryPoint.EnableDebugInfo)
            {
                Logs.LogMessage("Distance: " + distance);
            }
            
            direction.Normalize();//射击方向归一化

            float backBonusMulti = data.allowDirectionalBonus ? GetBackBonusMulti(dam_EnemyDamageBase, direction) : 1f; //获取后背加成倍率

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
                falloffMulti = 1f - (distance - falloff.x) / falloff.y; //衰减算法还有问题
            }
            if (EntryPoint.EnableDebugInfo)
            {
                Logs.LogMessage("FalloffMulti: " + falloffMulti);
            }

            float targetDamage = limb.TestDamageModifiers(weaponDamage, precisionMulti) * falloffMulti * backBonusMulti * boosterWeaponDamageMulti;
            targetDamage = Math.Min(dam_EnemyDamageBase.HealthMax, targetDamage);

            if (EntryPoint.EnableDebugInfo)
            {
                Logs.LogMessage(string.Format("damage:{0},targetDamage:{1}", damage, targetDamage));
            }

            bool flag2 = Math.Abs(damage - targetDamage) <= 0.02f;
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

            //去除y轴分量
            direction.y = 0f; 

            float angle = Vector3.Angle(enemyForward, direction);

            //大于90度不吃背伤，如果有realbackbonus以后再考虑
            return angle > 90f ? 1f : 2f;
        }
    }
}
