using System;
using StardewSignals.Events;

namespace StardewSignals
{
	public class EventsApi : IStardewSignals
	{
		ModEntry modEntry;

		public EventsApi(ModEntry modEntry)
		{
			this.modEntry = modEntry;
		}

		public event EventHandler<HeartLevelEventArgs> HeartLevelChanged
		{
			add  { modEntry._friendEvents.HeartLevelChanged += value; }
			remove { modEntry._friendEvents.HeartLevelChanged -= value; }
		}

		public event EventHandler<FriendshipEventArgs> FriendshipChanged
		{
			add  { modEntry._friendEvents.FriendshipChanged += value; }
			remove { modEntry._friendEvents.FriendshipChanged -= value; }
		}

		public event EventHandler<PetFriendshipEventArgs> PetFriendshipChanged
		{
			add  { modEntry._friendEvents.PetFriendshipChanged += value; }
			remove { modEntry._friendEvents.PetFriendshipChanged -= value; }
		}
	}
}