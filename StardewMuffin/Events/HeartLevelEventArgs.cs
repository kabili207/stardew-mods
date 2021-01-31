using System;
using StardewValley;

namespace StardewSignals.Events
{
	public class HeartLevelEventArgs : EventArgs
	{
		public Farmer Farmer;
		public NPC NPC;
		public int PreviousPoints;
		public int NewPoints;

		public HeartLevelEventArgs(Farmer farmer, NPC npc, Friendship friendship, int prevPoints)
		{
			Farmer = farmer;
			NPC = npc;
			PreviousPoints = prevPoints;
			NewPoints = friendship.Points;
		}
	}
}

