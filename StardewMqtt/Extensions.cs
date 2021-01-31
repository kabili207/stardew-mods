using System;
using System.Linq;
using System.Numerics;
namespace StardewMqtt
{
	public static class Extensions
	{
		private static readonly char[] _chars = { '0','1','2','3','4','5','6','7','8','9',
			'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
			'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'};
		
		public static string ToString(this int value, int @base)
		{
			return value.ToStringFast(_chars.Take(@base).ToArray());
		}

		/// <summary>
		/// An optimized method using an array as buffer instead of 
		/// string concatenation. This is faster for return values having 
		/// a length > 1.
		/// </summary>
		public static string ToStringFast(this int value, char[] baseChars)
		{
			// 32 is the worst cast buffer size for base 2 and int.MaxValue
			int i = 32;
			char[] buffer = new char[i];
			int targetBase= baseChars.Length;

			do
			{
				buffer[--i] = baseChars[value % targetBase];
				value = value / targetBase;
			}
			while (value > 0);

			char[] result = new char[32 - i];
			Array.Copy(buffer, i, result, 0, 32 - i);

			return new string(result);
		}
		
		public static string ToString(this ulong value, int @base)
		{
			return value.ToStringFast(_chars.Take(@base).ToArray());
		}

		/// <summary>
		/// An optimized method using an array as buffer instead of 
		/// string concatenation. This is faster for return values having 
		/// a length > 1.
		/// </summary>
		public static string ToStringFast(this ulong value, char[] baseChars)
		{
			// 32 is the worst cast buffer size for base 2 and int.MaxValue
			int i = 32;
			char[] buffer = new char[i];
			int targetBase= baseChars.Length;
			
			BigInteger bValue = value;

			do
			{
				buffer[--i] = baseChars[(int)(bValue % targetBase)];
				bValue = bValue / targetBase;
			}
			while (bValue > 0);

			char[] result = new char[32 - i];
			Array.Copy(buffer, i, result, 0, 32 - i);

			return new string(result);
		}
	}
}

