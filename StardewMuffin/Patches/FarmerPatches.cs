using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffinCore.Patches
{
    public static class FarmerPatches
    {
        public static void ApplyPatches(HarmonyInstance harmony)
        {
            // Farmer
            harmony.Patch(
               original: AccessTools.Method(typeof(Farmer), nameof(Farmer.changeFriendship)),
               prefix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.ChangeFriendship_Prefix)),
               postfix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.ChangeFriendship_Postfix))
            );
        }

        public static bool ChangeFriendship_Prefix(Farmer __instance, int amount, NPC n, out int __state)
        {
            __state = 0;
            if (__instance.friendshipData.ContainsKey(n?.Name))
            {
                __state = __instance.friendshipData[n.Name].Points;
            }
            return true;
        }

        public static void ChangeFriendship_Postfix(Farmer __instance, int amount, NPC n, int __state)
        {
            if (__instance.friendshipData.ContainsKey(n?.Name) && __state != __instance.friendshipData[n.Name].Points)
            {
                ModEntry.ThisMod._friendEvents.ProcessChange(__instance, n, __instance.friendshipData[n.Name], __state);
            }
        }
    }
}
