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
}
