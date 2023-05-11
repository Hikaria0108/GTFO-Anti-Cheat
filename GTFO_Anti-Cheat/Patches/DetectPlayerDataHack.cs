using Player;
using Hikaria.GTFO_Anti_Cheat.Utils;

namespace Hikaria.GTFO_Anti_Cheat.Patches
{
    internal class DetectPlayerDataHack : Patch
    {
        public override void Execute()
        {

        }

        public override string Name { get; } = "DetectPlayerDataHack";

        public static Patch Instance { get; private set; }

        public override void Initialize()
        {
            DetectPlayerDataHack.Instance = this;
        }

        private const string PatchName = "DetectPlayerDataHack";
    }
}
