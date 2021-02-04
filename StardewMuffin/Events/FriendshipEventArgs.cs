using System;
using StardewValley;
using StardewValley.Characters;

namespace MuffinCore.Events
{
	public class FriendshipEventArgs : EventArgs
	{
		public Farmer Farmer;
		public Character Character;
		public int PreviousPoints;
		public int NewPoints;

		public FriendshipEventArgs(Farmer farmer, Character character, int newPoints, int prevPoints)
		{
			Farmer = farmer;
			Character = character;
			PreviousPoints = prevPoints;
			NewPoints = newPoints;
		}
	}
}

