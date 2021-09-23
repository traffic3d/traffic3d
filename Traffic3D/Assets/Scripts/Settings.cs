using UnityEditor;

public static class Settings
{
    public static void SetBenchmark()
    {
#if UNITY_EDITOR
        EditorPrefs.SetBool("isBenchmark", true);
#endif
    }

    public static bool IsBenchmark()
    {
#if UNITY_EDITOR
        return EditorPrefs.GetBool("isBenchmark");
#else
        return false;
#endif
    }

    public static void SetRandomSeed(int inputSeed)
    {
#if UNITY_EDITOR
        EditorPrefs.SetInt("randomSeed", inputSeed);
#endif
    }

    public static bool GetRandomSeed(out int seed)
    {
#if UNITY_EDITOR
        seed = EditorPrefs.GetInt("randomSeed");
        return true;
#else
        seed = RandomNumberGenerator.DEFAULT_RANDOM_SEED;
        return false;
#endif
    }
}
