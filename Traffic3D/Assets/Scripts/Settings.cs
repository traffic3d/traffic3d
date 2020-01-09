using UnityEngine;

public static class Settings
{
    private static bool isBenchmark = false;
    private static bool headlessMode = false;

    public static void SetBenchmark()
    {
        isBenchmark = true;
    }

    public static bool IsBenchmark()
    {
        return isBenchmark;
    }

    public static void SetHeadlessMode(bool input)
    {
        Debug.Log("Headless Set To " + input);
        headlessMode = input;
    }

    public static bool IsHeadlessMode()
    {
        return headlessMode;
    }
}
