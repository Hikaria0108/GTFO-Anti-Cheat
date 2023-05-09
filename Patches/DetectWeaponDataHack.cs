using UnityEngine;
using Hikaria.GTFO_Anti_Cheat.Utils;
using BoosterImplants;
using SNetwork;
using CellMenu;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using GameData;

namespace Hikaria.GTFO_Anti_Cheat.Patches
{
    internal class DetectWeaponDataHack : Patch
    {
        public override void Execute()
        {
            
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
