using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core
{
	/// <summary>
	/// A centralized random number provider.
	/// </summary>
	public class RandomFactory
	{
		private Random _random = new();

		static RandomFactory()
		{
			Instance = new();
		}

		private RandomFactory()
		{
		}

		public static RandomFactory Instance { get; }

		/// <summary>
		/// Initializes a new instance of the System.Random class, using the specified seed value.
		/// </summary>
		/// <param name="seed">
		/// A number used to calculate a starting value for the pseudo-random number sequence.
		/// If a negative number is specified, the absolute value of the number is used.
		/// </param>
		public void Reseed(int seed)
		{
			_random = new Random(seed);
		}

		/// <summary>
		/// Returns a non-negative random integer.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that is greater than or equal to 0 and less than System.Int32.MaxValue.
		/// </returns>
		public int Next()
		{
			return _random.Next();
		}

		/// <summary>
		/// Returns a non-negative random integer that is less than the specified maximum.
		/// </summary>
		/// <param name="maxValue">
		/// The exclusive upper bound of the random number to be generated. maxValue must be greater than or equal to 0.
		/// </param>
		/// <returns>
		/// A 32-bit signed integer that is greater than or equal to 0, and less than maxValue;
		/// that is, the range of return values ordinarily includes 0 but not maxValue. However,
		/// if maxValue equals 0, maxValue is returned.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="maxValue"/> is less than 0.
		/// </exception>
		public int Next(int maxValue)
		{
			return _random.Next(maxValue);
		}

		/// <summary>
		/// Returns a random integer that is within a specified range.
		/// </summary>
		/// <param name="minValue">
		/// The inclusive lower bound of the random number returned.
		/// </param>
		/// <param name="maxValue">
		/// The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.
		/// </param>
		/// <returns>
		/// A 32-bit signed integer greater than or equal to minValue and less than maxValue;
		/// that is, the range of return values includes minValue but not maxValue. If minValue
		/// equals maxValue, minValue is returned.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
		/// </exception>
		public int Next(int minValue, int maxValue)
		{
			return _random.Next(minValue, maxValue);
		}

		/// <summary>
		/// Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.
		/// </summary>
		/// <returns>
		/// A double-precision floating point number that is greater than or equal to 0.0, and less than 1.0.
		/// </returns>
		public double NextDouble()
		{
			return _random.NextDouble();
		}
	}
}
