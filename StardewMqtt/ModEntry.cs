using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewSignals.Events;
using StardewModdingAPI.Enums;
using StardewValley.Characters;

namespace StardewMqtt
{
	public class ModEntry : Mod
	{
		IStardewSignals _signalsAPI;
		MqttConnection _mqtt = null;
		const string baseTopic = "stardew/";
		Dictionary<long, string> _playerTopics = new Dictionary<long, string>();


		public override void Entry(IModHelper helper)
		{
			Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
			Helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
			Helper.Events.Player.LevelChanged += Player_LevelChanged;
			Helper.Events.Display.MenuChanged += Display_MenuChanged;
			Helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
		}

		void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
		{
			try
			{
				_signalsAPI = Helper.ModRegistry.GetApi<IStardewSignals>("kabi-chan.StardewSignals");
			}
			catch (Exception ex)
			{
				Monitor.Log($"Could not load stardew signals API", LogLevel.Error);
			}

			if (_signalsAPI != null)
			{
				_signalsAPI.FriendshipChanged += Signals_FriendshipChanged;
				_signalsAPI.PetFriendshipChanged += Signals_PetFriendshipChanged;
			}
		}

		void Display_MenuChanged(object sender, MenuChangedEventArgs e)
		{

		}

		void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			if (_mqtt != null)
			{
				_mqtt.Close();
			}

			_mqtt = new MqttConnection(Monitor, "192.168.77.40");
			_mqtt.Open();

			var npcs = GetVillagers();
			var allFarmers = Game1.getAllFarmers();

			foreach (Farmer farmer in allFarmers)
			{
				InitFarmerTopic(farmer, npcs);
			}
		}

		private void InitFarmerTopic(Farmer farmer, IEnumerable<NPC> npcs)
		{
			ulong uMultiID = unchecked((ulong)farmer.UniqueMultiplayerID);
			string deviceID = $"{farmer.Name}-{uMultiID.ToString(42)}".Replace(' ', '-');

			Monitor.Log($"Name: {farmer.Name}");
			Monitor.Log($"Multi ID: {uMultiID}");
			Monitor.Log($"Dev ID: {uMultiID.ToString(42)}");

			string farmerBase = baseTopic + deviceID;

			_mqtt.Send(farmerBase + "/$state", "init", true);
			_mqtt.Send(farmerBase + "/$homie", "3.0", true);
			_mqtt.Send(farmerBase + "/$name", $"Stardew: {farmer.Name} - {farmer.farmName}", true);
			_mqtt.Send(farmerBase + "/$implementation", $"stardew-mqtt", true);

			List<string> friendProps = new List<string>();
			foreach (var npc in npcs)
			{
				int? level = farmer.tryGetFriendshipLevelForNPC(npc.Name);
				if (level.HasValue)
				{
					friendProps.Add(npc.Name);
					string npcTopic = farmerBase + "/friendship/" + npc.Name;
					DefineNodeProperty(npcTopic, $"Friendship Level - {npc.getName()}",
									   "integer", false, (level.Value).ToString());
				}
			}

			_mqtt.Send(farmerBase + "/friendship/$name", "Friendship Levels", true);
			_mqtt.Send(farmerBase + "/friendship/$type", "NPC", true);
			_mqtt.Send(farmerBase + "/friendship/$properties", string.Join(",", friendProps), true);

			string skillBase = farmerBase + "/skills/";
			DefineNodeProperty(skillBase + "combat", $"Skill Level - Combat", "integer", false, farmer.CombatLevel.ToString());
			DefineNodeProperty(skillBase + "farming", $"Skill Level - Farming", "integer", false, farmer.FarmingLevel.ToString());
			DefineNodeProperty(skillBase + "fishing", $"Skill Level - Fishing", "integer", false, farmer.FishingLevel.ToString());
			DefineNodeProperty(skillBase + "foraging", $"Skill Level - Foraging", "integer", false, farmer.ForagingLevel.ToString());
			DefineNodeProperty(skillBase + "mining", $"Skill Level - Mining", "integer", false, farmer.MiningLevel.ToString());

			_mqtt.Send(farmerBase + "/skills/$name", "Skill Levels", true);
			_mqtt.Send(farmerBase + "/skills/$type", "Skills", true);
			_mqtt.Send(farmerBase + "/skills/$properties", "combat,farming,fishing,foraging,mining", true);

			_mqtt.Send(farmerBase + "/$nodes", "friendship,skills", true);
			_mqtt.Send(farmerBase + "/$state", "ready", true);

			_playerTopics[farmer.UniqueMultiplayerID] = farmerBase;
		}

		private void DefineNodeProperty(string propTopic, string name, string dataType, bool settable, string value)
		{
			_mqtt.Send(propTopic + "/$name", name, true);
			_mqtt.Send(propTopic + "/$datatype", dataType, true);
			_mqtt.Send(propTopic + "/$settable", settable.ToString().ToLower(), true);
			_mqtt.Send(propTopic, value, false);
		}

		private string GetFarmerBaseTopic(Farmer farmer)
		{
			return _playerTopics[farmer.UniqueMultiplayerID];
		}

		void GameLoop_OneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
		{
			// skip if save not loaded yet
			if (!Context.IsWorldReady)
				return;
		}

		void Player_LevelChanged(object sender, LevelChangedEventArgs e)
		{
			var skill = e.Skill;
			var level = e.NewLevel;

			if (!_playerTopics.ContainsKey(e.Player.UniqueMultiplayerID))
			{
				var a = e.Player.getChildren();
				InitFarmerTopic(e.Player, GetVillagers());
			}

			Monitor.Log($"{e.Player.Name}'s {skill} level changed to {level}");
			string npcTopic = GetFarmerBaseTopic(e.Player) + "/skills/" + e.Skill.ToString().ToLower();
			_mqtt.Send(npcTopic, e.NewLevel.ToString(), false);
		}

		void Signals_FriendshipChanged(object sender, FriendshipEventArgs e)
		{
			if (!_playerTopics.ContainsKey(e.Farmer.UniqueMultiplayerID))
			{
				InitFarmerTopic(e.Farmer, GetVillagers());
			}
			Monitor.Log($"{e.Farmer.Name}'s friendship with {e.NPC.getName()} changed from {e.PreviousPoints} to {e.NewPoints}");

			string npcTopic = GetFarmerBaseTopic(e.Farmer) + "/friendship/" + e.NPC.Name;
			_mqtt.Send(npcTopic, e.NewPoints.ToString(), false);
		}

		void Signals_PetFriendshipChanged(object sender, PetFriendshipEventArgs e)
		{
			if (!_playerTopics.ContainsKey(e.Farmer.UniqueMultiplayerID))
			{
				InitFarmerTopic(e.Farmer, GetVillagers());
			}
			Monitor.Log($"{e.Farmer.Name}'s friendship with {e.Pet.getName()} changed from {e.PreviousPoints} to {e.NewPoints}");

			string npcTopic = GetFarmerBaseTopic(e.Farmer) + "/pet/" + e.Pet.Name;
			_mqtt.Send(npcTopic, e.NewPoints.ToString(), false);
		}

		private List<NPC> GetVillagers(bool onlyDatable = false)
		{
			List<NPC> datables = new List<NPC>();
			try
			{
				foreach (NPC character in Utility.getAllCharacters())
				{
					if ((character.isVillager() || character is Child) &&
						(!onlyDatable || character.datable.Value))
					{
						datables.Add(character);
					}
				}
			}
			catch (Exception ex)
			{
				Monitor.Log("Error getting NPCs: " + ex.Message, LogLevel.Error);
			}

			datables.Sort();
			datables.Reverse();
			return datables;
		}
	}
}

