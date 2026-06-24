namespace ECMABasic.Domain;

/// <summary>
/// Random number generator for BASIC RND function.
/// ECMA-55 requires repeatability (same seed = same sequence).
/// </summary>
public interface IRandomNumberGenerator
{
	/// <summary>
	/// Returns a random number in range [0, 1) per ECMA-55.
	/// </summary>
	public double NextDouble();

	/// <summary>
	/// Returns a random integer in range [0, maxValue).
	/// </summary>
	/// <param name="maxValue">Exclusive upper bound.</param>
	public int Next(int maxValue);

	/// <summary>
	/// Re-seeds the RNG with a deterministic seed (for testing).
	/// </summary>
	/// <param name="seed">Seed value for deterministic sequence.</param>
	public void Reseed(int seed);

	/// <summary>
	/// Re-seeds the RNG with an unpredictable value (for RANDOMIZE statement).
	/// ECMA55-RND-001: RANDOMIZE generates unpredictable starting point.
	/// </summary>
	public void Randomize();
}
