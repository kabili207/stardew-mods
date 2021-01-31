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

namespace StardewSignals
{
	public class ModEntry : Mod
	{
		public int ForagingLevel { get; private set; }
		private EventsApi API;
		internal FriendshipEvents _friendEvents;

		private Dictionary<long, Dictionary<string, int>> _friendshipPoints =
			new Dictionary<long, Dictionary<string, int>>();

		private Dictionary<long, int> _petFriendship = new Dictionary<long, int>();

		public ModEntry()
		{
			_friendEvents = new FriendshipEvents();
		}

		public override object GetApi()
		{
			return API = new EventsApi(this);
		}

		public override void Entry(IModHelper helper)
		{
			Helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;

			Helper.Events.GameLoop.UpdateTicked += GameLoop_TenthSecondUpdateTicked;
		}

		void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			//ForagingLevel = Game1.player.ForagingLevel;

		}

		void GameLoop_TenthSecondUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (e.IsMultipleOf(6))
			{
				// skip if save not loaded yet
				if (!Context.IsWorldReady)
					return;

				Friendship friendship;
				try
				{
					foreach (Farmer farmer in Game1.getAllFarmers())
					{
						Pet pet = GetPet(farmer);

						if (pet != null)
						{
							int newFriendship = pet.friendshipTowardFarmer;
							int oldFrienship = 0;
							_petFriendship.TryGetValue(farmer.UniqueMultiplayerID, out oldFrienship);
							if (newFriendship > oldFrienship)
							{
								_friendEvents.ProcessChange(farmer, pet, newFriendship, oldFrienship);
								_petFriendship[farmer.UniqueMultiplayerID] = newFriendship;
							}
						}

						if (!_friendshipPoints.ContainsKey(farmer.UniqueMultiplayerID))
						{
							_friendshipPoints[farmer.UniqueMultiplayerID] = new Dictionary<string, int>();
						}
						var friendPoints = _friendshipPoints[farmer.UniqueMultiplayerID];

						foreach (NPC npc in GetVillagers())
						{
							if (!farmer.friendshipData.ContainsKey(npc.Name))
							{
								continue;
							}

							friendship = farmer.friendshipData[npc.Name];
							var newPoints = friendship.Points;

							int prevPoints = 0;
							if (friendPoints.TryGetValue(npc.Name, out prevPoints))
							{
								if (prevPoints != newPoints)
								{
									//Monitor.Log($"{npcName}'s friendship changed from {prevPoints} to {newPoints}");
									_friendEvents.ProcessChange(farmer, npc, friendship, prevPoints);
								}
							}

							friendPoints[npc.Name] = newPoints;
							//Stats[npc.Name] = new FriendshipStats(farmer, npc, friendship);
						}
					}
				}
				catch (Exception ex)
				{
					Monitor.Log($"Error updating friendship values: {ex.Message}", LogLevel.Error);
				}
			}
		}

		private Pet GetPet(Farmer farmer)
		{
			try
			{
				return (Pet)(Game1.getCharacterFromName(farmer.getPetName()));
			}
			catch
			{
				return null;
			}
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

