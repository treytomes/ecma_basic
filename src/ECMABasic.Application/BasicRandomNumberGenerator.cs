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
	private bool _isDeterministic;
	private const int DEFAULT_SEED = 42; // Fixed for repeatability

	public BasicRandomNumberGenerator()
	{
		_random = new Random(DEFAULT_SEED);
		_isDeterministic = false;
	}

	public double NextDouble() => _random.NextDouble();

	public int Next(int maxValue) => _random.Next(maxValue);

	public void Reseed(int seed)
	{
		_random = new Random(seed);
		_isDeterministic = true; // Mark as deterministic when explicitly seeded
	}

	public void Randomize()
	{
		// ECMA55-RND-001: Generate unpredictable starting point
		// Use time-based seed for unpredictability
		// BUT: If Reseed() was called (test mode), ignore RANDOMIZE to preserve determinism
		if (!_isDeterministic)
		{
			_random = new Random();
		}
		// In deterministic mode (after Reseed), RANDOMIZE is a no-op
		// This prevents tests from becoming flaky
	}
}
