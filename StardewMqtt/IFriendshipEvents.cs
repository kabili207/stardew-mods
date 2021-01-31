using System;
namespace StardewMqtt
{
	public interface IFriendshipEvents
	{
		event EventHandler<EventArgs> PointsChanged;
		event EventHandler<EventArgs> HeartLevelChanged;
	}
}

