using System;
using StardewValley;
using StardewValley.Characters;

namespace StardewSignals.Events
{
	public class FriendshipEventArgs : EventArgs
	{
		public Farmer Farmer;
		public NPC NPC;
		public int PreviousPoints;
		public int NewPoints;

		public FriendshipEventArgs(Farmer farmer, NPC npc, Friendship friendship, int prevPoints)
		{
			Farmer = farmer;
			NPC = npc;
			PreviousPoints = prevPoints;
			NewPoints = friendship.Points;
		}
	}
	
	public class PetFriendshipEventArgs : EventArgs
	{
		public Farmer Farmer;
		public Pet Pet;
		public int PreviousPoints;
		public int NewPoints;

		public PetFriendshipEventArgs(Farmer farmer, Pet pet, int newPoints, int prevPoints)
		{
			Farmer = farmer;
			Pet = pet;
			PreviousPoints = prevPoints;
			NewPoints = newPoints;
		}
	}
}

