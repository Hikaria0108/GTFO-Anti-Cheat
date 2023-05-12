using GameData;
using Gear;
using Hikaria.GTFO_Anti_Cheat.Utils;
using System.Collections.Generic;
using Il2CppSystem.Text.RegularExpressions;
using SNetwork;
using Gear;
using Player;
using static Gear.GearIDRange;

namespace Hikaria.GTFO_Anti_Cheat.Managers
{
    internal class WeaponDataManager
    {
        public static bool CheckIsValidWeaponGearIDRangeDataForPlayer(SNet_Player player)
        {
            foreach (GearIDRange gearIDRange in GearManager.Current.m_gearPerSlot[player.PlayerSlotIndex()]) 
            {
                string gearJson = gearIDRange.ToJSON();

                string pattern = "(?<=Comps\":)(.*?)(?=,\"MatTrans\")";
                Match comps = Regex.Match(gearJson, pattern);

                string pattern2 = "(?<=Name\":\")(.*?)(?=\")";
                Match Name = Regex.Match(gearJson, pattern2);

                string pattern3 = "(?<=data\":\")(.*?)(?=\"})";
                Match publicName = Regex.Match(gearJson, pattern3);

                string gear = Name.Value + comps.Value + publicName.Value;

                if (!compsHashDict.ContainsKey(gear.GetHashString(HashHelper.HashType.MD5)))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CheckIsValidWeaponGearIDRangeData(GearIDRange gearIDRange)
        {
            string gearJson = gearIDRange.ToJSON();

            string pattern = "(?<=Comps\":)(.*?)(?=,\"MatTrans\")";
            Match comps = Regex.Match(gearJson, pattern);

            string pattern2 = "(?<=Name\":\")(.*?)(?=\")";
            Match Name = Regex.Match(gearJson, pattern2);

            string pattern3 = "(?<=data\":\")(.*?)(?=\"})";
            Match publicName = Regex.Match(gearJson, pattern3);

            string gear = Name.Value + comps.Value + publicName.Value;

            return compsHashDict.ContainsKey(gear.GetHashString(HashHelper.HashType.MD5));
        }

        public static void LoadData()
        {
            foreach (PlayerOfflineGearDataBlock block in GameDataBlockBase<PlayerOfflineGearDataBlock>.GetAllBlocksForEditor())
            {
                string gearJson = block.GearJSON;
                string pattern = "(?<=Comps\":)(.*?)(?=,\"MatTrans\")";
                Match comps = Regex.Match(gearJson, pattern);
                string pattern2 = "(?<=Name\":\")(.*?)(?=\")";
                Match Name = Regex.Match(gearJson, pattern2);
                string pattern3 = "(?<=data\":\")(.*?)(?=\"})";
                Match publicName = Regex.Match(gearJson, pattern3);
                string gear = Name.Value + comps.Value + publicName.Value;
                compsHashDict.Add(gear.GetHashString(HashHelper.HashType.MD5), gearJson);
            }
        }

        private static Dictionary<string, string> compsHashDict = new Dictionary<string, string>();
    }
}
