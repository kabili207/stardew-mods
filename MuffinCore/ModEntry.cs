using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;
using Harmony;
using MuffinCore.Patches;

namespace MuffinCore
{
	public class ModEntry : Mod
	{
		internal static ModEntry ThisMod;
		private MuffinApi API;
		internal FriendshipEvents _friendEvents;

		public ModEntry()
		{
			_friendEvents = new FriendshipEvents();
			ThisMod = this;
		}

		private void ApplyPatches()
        {
			var harmony = HarmonyInstance.Create(ModManifest.UniqueID);
			FarmerPatches.ApplyPatches(harmony);
			FarmAnimalPatches.ApplyPatches(harmony);
			PetPatches.ApplyPatches(harmony);
			ShearsPatches.ApplyPatches(harmony);
			MilkPailPatches.ApplyPatches(harmony);
		}

		public override object GetApi()
		{
			return API = new MuffinApi(this);
		}

		public override void Entry(IModHelper helper)
		{
			ApplyPatches();
			Helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
		}

		void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			//ForagingLevel = Game1.player.ForagingLevel;
		}

		private List<NPC> GetVillagers(bool onlyDatable = false)
		{
			List<NPC> datables = new List<NPC>();
			try
			{
				foreach (NPC character in Utility.getAllCharacters())
				{
					if ((character.isVillager() || character is Child) && (!onlyDatable || character.datable.Value))
					{
						datables.Add(character);
					}
				}
			}
			catch (Exception ex)
			{
				Monitor.Log($"Error loading NPCs: {ex.Message}", LogLevel.Error);
			}
			datables.Sort();
			datables.Reverse();
			return datables;
		}
	}
}

