using Harmony;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffinCore.Patches
{
    public static class PetPatches
    {
        public static void ApplyPatches(HarmonyInstance harmony)
        {

            harmony.Patch(
               original: AccessTools.Method(typeof(Pet), nameof(Pet.checkAction)),
               prefix: new HarmonyMethod(typeof(PetPatches), nameof(PetPatches.CheckAction_Prefix)),
               postfix: new HarmonyMethod(typeof(PetPatches), nameof(PetPatches.CheckAction_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(Pet), nameof(Pet.dayUpdate)),
               prefix: new HarmonyMethod(typeof(PetPatches), nameof(PetPatches.DayUpdate_Prefix)),
               postfix: new HarmonyMethod(typeof(PetPatches), nameof(PetPatches.DayUpdate_Postfix))
            );
        }

        private static bool ChangeFriendship_Prefix(Pet pet, out int previousValue)
        {
            previousValue = pet.friendshipTowardFarmer.Value;
            return true;
        }

        private static void ChangeFriendship_Postfix(Pet pet, Farmer who, int previousValue)
        {
            if (previousValue != pet.friendshipTowardFarmer.Value)
            {
                if (who != null)
                {
                    ModEntry.ThisMod._friendEvents.ProcessChange(who, pet, pet.friendshipTowardFarmer.Value, previousValue);
                }
                else
                {
                    foreach (Farmer farmer in Game1.getAllFarmers())
                    {
                        ModEntry.ThisMod._friendEvents.ProcessChange(farmer, pet, pet.friendshipTowardFarmer.Value, previousValue);
                    }
                }
            }
        }

        public static bool CheckAction_Prefix(Pet __instance, out int __state) => ChangeFriendship_Prefix(__instance, out __state);

        public static void CheckAction_Postfix(Pet __instance, Farmer who, int __state) => ChangeFriendship_Postfix(__instance, who, __state);

        public static bool DayUpdate_Prefix(Pet __instance, out int __state) => ChangeFriendship_Prefix(__instance, out __state);

        public static void DayUpdate_Postfix(Pet __instance, int __state) => ChangeFriendship_Postfix(__instance, null, __state);
    }
}
