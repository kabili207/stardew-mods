using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffinCore.Patches
{
    public static class FarmAnimalPatches
    {
        public static void ApplyPatches(HarmonyInstance harmony)
        {
            harmony.Patch(
               original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.pet)),
               prefix: new HarmonyMethod(typeof(FarmAnimalPatches), nameof(FarmAnimalPatches.Pet_Prefix)),
               postfix: new HarmonyMethod(typeof(FarmAnimalPatches), nameof(FarmAnimalPatches.Pet_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.Eat)),
               prefix: new HarmonyMethod(typeof(FarmAnimalPatches), nameof(FarmAnimalPatches.Eat_Prefix)),
               postfix: new HarmonyMethod(typeof(FarmAnimalPatches), nameof(FarmAnimalPatches.Eat_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.dayUpdate)),
               prefix: new HarmonyMethod(typeof(FarmAnimalPatches), nameof(FarmAnimalPatches.DayUpdate_Prefix)),
               postfix: new HarmonyMethod(typeof(FarmAnimalPatches), nameof(FarmAnimalPatches.DayUpdate_Postfix))
            );
        }

        private static bool ChangeFriendship_Prefix(FarmAnimal farmAnimal, out int previousValue)
        {
            previousValue = farmAnimal.friendshipTowardFarmer.Value;
            return true;
        }

        private static void ChangeFriendship_Postfix(FarmAnimal farmAnimal, Farmer who, int previousValue)
        {
            if (previousValue != farmAnimal.friendshipTowardFarmer.Value)
            {
                if (who != null)
                {
                    ModEntry.ThisMod._friendEvents.ProcessChange(who, farmAnimal, farmAnimal.friendshipTowardFarmer.Value, previousValue);
                }
                else
                {
                    foreach (Farmer farmer in Game1.getAllFarmers())
                    {
                        ModEntry.ThisMod._friendEvents.ProcessChange(farmer, farmAnimal, farmAnimal.friendshipTowardFarmer.Value, previousValue);
                    }
                }
            }
        }

        public static bool Pet_Prefix(FarmAnimal __instance, out int __state) => ChangeFriendship_Prefix(__instance, out __state);

        public static void Pet_Postfix(FarmAnimal __instance, Farmer who, int __state) => ChangeFriendship_Postfix(__instance, who, __state);

        public static bool DayUpdate_Prefix(FarmAnimal __instance, out int __state) => ChangeFriendship_Prefix(__instance, out __state);

        public static void DayUpdate_Postfix(FarmAnimal __instance, int __state) => ChangeFriendship_Postfix(__instance, null, __state);

        public static bool Eat_Prefix(FarmAnimal __instance, out int __state) => ChangeFriendship_Prefix(__instance, out __state);

        public static void Eat_Postfix(FarmAnimal __instance, int __state) => ChangeFriendship_Postfix(__instance, null, __state);
    }
}
