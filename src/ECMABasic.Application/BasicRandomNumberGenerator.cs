using System;
using ECMABasic.Domain;

namespace ECMABasic.Application;

/// <summary>
/// ECMA-55 compliant random number generator.
/// Uses fixed seed (42) for repeatability per ECMA55-FUN-008.
/// </summary>
public class BasicRandomNumberGenerator : IRandomNumberGenerator
{
	private Random _random;
	private const int DEFAULT_SEED = 42; // Fixed for repeatability

	public BasicRandomNumberGenerator()
	{
		_random = new Random(DEFAULT_SEED);
	}

	public double NextDouble() => _random.NextDouble();

	public int Next(int maxValue) => _random.Next(maxValue);

	public void Reseed(int seed)
	{
		_random = new Random(seed);
	}
}
