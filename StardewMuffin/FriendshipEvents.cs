using System;
using StardewValley;
using MuffinCore.Events;
using StardewValley.Characters;

namespace MuffinCore
{
	public class FriendshipEvents : IMuffinApi
	{
		public event EventHandler<FriendshipEventArgs> FriendshipChanged;

		public void ProcessChange(Farmer farmer, Character character, Friendship friendship, int prevPoints)
		{
			ProcessChange(farmer, character, friendship.Points, prevPoints);
		}

		public void ProcessChange(Farmer farmer, Character character, int newPoints, int prevPoints)
		{
			FriendshipChanged?.Invoke(this, new FriendshipEventArgs(farmer, character, newPoints, prevPoints));
		}
	}
}

