using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MuffinCore.Patches
{
    internal static class ShearsPatches
    {
        public static void ApplyPatches(HarmonyInstance harmony)
        {
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Tools.Shears), nameof(StardewValley.Tools.Shears.DoFunction)),
               prefix: new HarmonyMethod(typeof(ShearsPatches), nameof(ShearsPatches.DoFunction_Prefix)),
               postfix: new HarmonyMethod(typeof(ShearsPatches), nameof(ShearsPatches.DoFunction_Postfix))
            );
        }

        public static bool DoFunction_Prefix(FarmAnimal ___animal,  out int __state)
        {
            __state = ___animal?.friendshipTowardFarmer?.Value ?? 0;
            return true;
        }

        public static void DoFunction_Postfix(FarmAnimal ___animal, Farmer who, int __state)
        {
            if (___animal != null && __state != ___animal.friendshipTowardFarmer.Value)
            {
                if (who != null)
                {
                    ModEntry.ThisMod._friendEvents.ProcessChange(who, ___animal, ___animal.friendshipTowardFarmer.Value, __state);
                }
                else
                {
                    foreach (Farmer farmer in Game1.getAllFarmers())
                    {
                        ModEntry.ThisMod._friendEvents.ProcessChange(farmer, ___animal, ___animal.friendshipTowardFarmer.Value, __state);
                    }
                }
            }
        }
    }
}
