using UnityEngine;
using Hikaria.GTFO_Anti_Cheat.Utils;
using SNetwork;
using Player;
using Gear;

namespace Hikaria.GTFO_Anti_Cheat.Patches
{
    internal class DetectDamangeHack : Patch
    {
        public override void Execute()
        {
            base.PatchMethod<Dam_EnemyDamageBase>("ReceiveBulletDamage", Patch.PatchType.Prefix, null, null, null);
            base.PatchMethod<Dam_EnemyDamageBase>("ReceiveMeleeDamage", Patch.PatchType.Prefix, null, null, null);
        }

        public override string Name { get; } = "DetectDamangeHack";

        public static Patch Instance { get; private set; }

        public override void Initialize()
        {
            DetectDamangeHack.Instance = this;
        }

        private static void Dam_EnemyDamageBase__ReceiveBulletDamage__Prefix(Dam_EnemyDamageBase __instance, pBulletDamageData data)
        {
            float distance = Vector3.Distance(__instance.DamageTargetPos, data.localPosition.vector.Value);
            Logs.LogMessage(string.Format("damage:{0},LimbID:{1},precisionMulti:{2},staggerMulti:{3},allowDirectionBonus:{4},distance:{5}", data.damage.internalValue, data.limbID, data.precisionMulti.internalValue, data.staggerMulti.internalValue, data.allowDirectionalBonus, distance));
            float damange = 0;
            bool isWeakSpot = false;
            bool isArmorSpot = false;
            Dam_EnemyDamageLimb limb;
            foreach (Dam_EnemyDamageLimb _limb in __instance.DamageLimbs)
            {
                if (_limb.m_limbID == data.limbID)
                {
                    limb = _limb;
                    isWeakSpot = limb.m_type == eLimbDamageType.Weakspot;
                    isArmorSpot = limb.m_type == eLimbDamageType.Armor;
                }
            }
            IReplicator replicator;
            data.source.pRep.TryGetID(out replicator);
            int PlayerSlotIndex = replicator.OwningPlayer.PlayerSlotIndex();
            PlayerAgent player;
            PlayerManager.TryGetPlayerAgent(ref PlayerSlotIndex, out player);            
        }

        private static void Dam_EnemyDamageBase__ReceiveMeleeDamage__Prefix(Dam_EnemyDamageBase __instance, pFullDamageData data)
        {

        }

        private const string PatchName = "DetectDamangeHack";


        private static bool IsValidBulletDamage(SNet_Player player, pBulletDamageData data)
        {
            return true;
        }
    }
}
