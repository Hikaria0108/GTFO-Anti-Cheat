using Hikaria.GTFO_Anti_Cheat.Utils;

namespace Hikaria.GTFO_Anti_Cheat.Patches
{
    internal class DetectBugHack : Patch
    {
        public override void Execute()
        {

        }

        public override string Name { get; } = "DetectBugHack";

        public static Patch Instance { get; private set; }

        public override void Initialize()
        {
            DetectBugHack.Instance = this;
        }

        private const string PatchName = "DetectBugHack";
    }
}
