using Player;
using Hikaria.GTFO_Anti_Cheat.Utils;

namespace Hikaria.GTFO_Anti_Cheat.Patches
{
    internal class DetectPlayerBackpackDataHack : Patch
    {
        public override void Execute()
        {

        }

        private static void PlayerBackpackManager____Postfix()
        {

        }

        public override string Name { get; } = "DetectPlayerBackpackDataHack";

        public static Patch Instance { get; private set; }

        public override void Initialize()
        {
            DetectPlayerBackpackDataHack.Instance = this;
        }

        private const string PatchName = "DetectPlayerBackpackDataHack";
    }
}
