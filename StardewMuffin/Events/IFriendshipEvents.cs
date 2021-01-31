using System;

namespace StardewSignals.Events
{
	public interface IFriendshipEvents
	{
		event EventHandler<FriendshipEventArgs> FriendshipChanged;
		event EventHandler<PetFriendshipEventArgs> PetFriendshipChanged;
		event EventHandler<HeartLevelEventArgs> HeartLevelChanged;
	}
}

