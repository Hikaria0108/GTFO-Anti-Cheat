using Player;
using UnityEngine;
using Hikaria.GTFO_Anti_Cheat.Utils;

namespace Hikaria.GTFO_Anti_Cheat.Patches
{
    internal class LocalPlayerPatch : Patch
    {
        public override void Execute()
        {
            base.PatchMethod<LocalPlayerAgent>("Setup", Patch.PatchType.Postfix, null, null, null);
        }

        public override string Name { get; } = "LocalPlayerPatch";

        public static Patch Instance { get; private set; }

        public override void Initialize()
        {
            LocalPlayerPatch.Instance = this;
        }

        private static void LocalPlayerAgent__Setup__Postfix(LocalPlayerAgent __instance)
        {
            GameObject gameObject = __instance.gameObject;
            if (gameObject.GetComponent<ChatManager>() == null)
            {
                gameObject.AddComponent<ChatManager>();
            }
            if (gameObject.GetComponent<GameEventLogManager>() == null)
            {
                gameObject.AddComponent<GameEventLogManager>();
            }
        }

        private const string PatchName = "LocalPlayerPatch";
    }
}
