using UnityEngine;

/// <summary>
/// The random number generator is used to centralise a System.Random object and ensure the simulation is repeatable.
/// The RandomNumberGenerator class implements the singleton pattern.
///
/// Note that the System.Random object is not thread safe so use a synchronization object
/// to ensure that only one thread can access the random number generator at a time.
///
/// Find more information about the System.Random class here:
/// https://docs.microsoft.com/en-us/dotnet/api/system.random?view=net-5.0
/// </summary>
public class RandomNumberGenerator
{
    public const int DEFAULT_RANDOM_SEED = 123;
    private static int seed = DEFAULT_RANDOM_SEED;
    private static RandomNumberGenerator instance;
    private System.Random random;

    public static RandomNumberGenerator GetInstance()
    {
        if (instance == null)
        {
            instance = new RandomNumberGenerator();
        }
        return instance;
    }

    public static void ReloadInstance()
    {
        instance = null;
        GetInstance();
    }

    public static int GetSeed()
    {
        if (Settings.GetRandomSeed(out int settingSeed))
        {
            seed = settingSeed;
            return seed;
        }
        else
        {
            return seed;
        }
    }

    private RandomNumberGenerator()
    {
        Random.InitState(GetSeed());
        random = new System.Random(GetSeed());
    }

    /// <summary>
    /// Returns the next random integer within the bounds of 0 (inclusive) and bound (exclusive).
    /// </summary>
    /// <param name="bound">The highest bound for the range to use</param>
    /// <returns>Returns the next random integer within the bounds of 0 (inclusive) and bound (exclusive).</returns>
    public int NextInt(int bound)
    {
        return random.Next(bound);
    }

    /// <summary>
    /// Returns the next random float between 0 and 1 inclusive.
    /// </summary>
    /// <returns>Returns the next random float between 0 and 1 inclusive.</returns>
    public float NextFloat()
    {
        return (float)random.NextDouble();
    }

    /// <summary>
    /// Return a random integer between min (inclusive) and max (exclusive)
    /// </summary>
    /// <param name="min">min number in range</param>
    /// <param name="max">max number in range</param>
    /// <returns>Return a random integer between min (inclusive) and max (exclusive)</returns>
    public int Range(int min, int max)
    {
        return random.Next(min, max);
    }

    /// <summary>
    /// Return a random float between min (inclusive) and max (inclusive)
    /// </summary>
    /// <param name="min">min number in range</param>
    /// <param name="max">max number in range</param>
    /// <returns>Return a random float between min (inclusive) and max (inclusive)</returns>
    public float Range(float min, float max)
    {
        return (float)(min + (random.NextDouble() * (max - min)));
    }

}
