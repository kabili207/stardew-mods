using MuffinCore.Events;
using System;

namespace MuffinCore
{
	public interface IMuffinApi
	{
		event EventHandler<FriendshipEventArgs> FriendshipChanged;
	}

	public class MuffinApi : IMuffinApi
	{
		private readonly ModEntry modEntry;

		public MuffinApi(ModEntry modEntry)
		{
			this.modEntry = modEntry;
		}

		public event EventHandler<FriendshipEventArgs> FriendshipChanged
		{
			add  { modEntry._friendEvents.FriendshipChanged += value; }
			remove { modEntry._friendEvents.FriendshipChanged -= value; }
		}
	}
}