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
    public class MilkPailPatches
    {
        public static void ApplyPatches(HarmonyInstance harmony)
        {
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Tools.MilkPail), nameof(StardewValley.Tools.MilkPail.beginUsing)),
               transpiler: new HarmonyMethod(typeof(MilkPailPatches), nameof(MilkPailPatches.BeginUsing_Transpiler))
            );
        }

        public static int GetFriendship(FarmAnimal animal)
        {
            return animal.friendshipTowardFarmer;
        }
        public static void CheckFriendship(FarmAnimal animal, int previous)
        {
            if (previous != animal.friendshipTowardFarmer)
            {
                foreach (Farmer farmer in Game1.getAllFarmers())
                {
                    ModEntry.ThisMod._friendEvents.ProcessChange(farmer, animal, animal.friendshipTowardFarmer.Value, previous);
                }
            }
        }

        internal static IEnumerable<CodeInstruction> BeginUsing_Transpiler(MethodBase original, ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            int beforeIndex = -1;
            int afterIndex = -1;

            for (var i = 0; i < codes.Count - 2; i++)
            {
                if (beforeIndex == -1 &&
                    codes[i].opcode == OpCodes.Ldarg_0 &&
                    codes[i + 1].opcode == OpCodes.Ldfld && ((FieldInfo)codes[i + 1].operand).Name == "animal" &&
                    codes[i + 2].opcode == OpCodes.Brfalse)
                {
                    beforeIndex = i + 3;
                }

                if (beforeIndex > -1 &&
                    codes[i].opcode == OpCodes.Ldarg_S && codes[i].operand.Equals(4) &&
                    codes[i + 1].opcode == OpCodes.Callvirt && ((MethodInfo)codes[i + 1].operand).Name == nameof(Character.Halt))
                {
                    afterIndex = i;
                    break;
                }
            }

            if (beforeIndex >= 0 && afterIndex >= beforeIndex)
            {
                LocalBuilder previousValueLocal = generator.DeclareLocal(typeof(int));
                codes.InsertRange(beforeIndex, new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0), // this
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Tools.MilkPail), "animal")),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MilkPailPatches), nameof(GetFriendship))),
                    new CodeInstruction(OpCodes.Stloc, previousValueLocal)
                });

                codes.InsertRange(afterIndex, new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0), // this
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Tools.MilkPail), "animal")),
                    new CodeInstruction(OpCodes.Ldloc, previousValueLocal), // Local gameObj
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MilkPailPatches), nameof(CheckFriendship)))
                });
            }
            return codes;
        }
    }
}
