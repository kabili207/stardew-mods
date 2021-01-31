using System;
using StardewValley;
using StardewValley.Characters;
using Microsoft.Xna.Framework.Graphics;

namespace StardewMuffin
{
	internal enum Eligibility
	{
		Ineligible,
		Bachelor,
		Bechelorette
	}

	internal class FriendshipStats
	{
		// Contants
		private const int PointsPerLvl = 250;
		private const int MaxPoints = 2500;

		// Instance Variables
		public FriendshipStatus Status;
		public string Name;
		public int Level { get { return (int)Math.Floor(LevelAsDecimal); } }
		public decimal LevelAsDecimal;
		public int Points;
		public int ToNextLevel;
		public int GiftsThisWeek;
		//public Icons.Portrait Portrait;
		//private ModConfig.DatableType DatingType;

		// Methods
		public FriendshipStats(Farmer player, NPC npc, Friendship friendship)
		{
			if (npc.datable.Value)
			{
				Name = npc.displayName;
				Status = friendship.Status;
				Points = friendship.Points;
				LevelAsDecimal = GetLevel(Points);

				ToNextLevel = 250 - (Points % PointsPerLvl);
				GiftsThisWeek = friendship.GiftsThisWeek;
				//this.Portrait = new Icons.Portrait(npc);
			}
		}

		private decimal GetLevel(int points)
		{
			if (Points >= MaxPoints)
			{
				return 10;
			}
			return (decimal)Points / PointsPerLvl;
		}
	}
}

