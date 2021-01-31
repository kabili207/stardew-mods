using System;
using StardewValley;
using StardewSignals.Events;
using StardewValley.Characters;

namespace StardewSignals
{
	public class FriendshipEvents : IFriendshipEvents
	{
		public event EventHandler<HeartLevelEventArgs> HeartLevelChanged;
		public event EventHandler<FriendshipEventArgs> FriendshipChanged;
		public event EventHandler<PetFriendshipEventArgs> PetFriendshipChanged;
		
		public void ProcessChange(Farmer farmer, NPC npc, Friendship friendship, int prevPoints)
		{
			FriendshipChanged?.Invoke(this, new FriendshipEventArgs(farmer, npc, friendship, prevPoints));
		}
		
		public void ProcessChange(Farmer farmer, Pet pet, int newPoints, int prevPoints)
		{
			PetFriendshipChanged?.Invoke(this, new PetFriendshipEventArgs(farmer, pet, newPoints, prevPoints));
		}
	}
}

