using System;

public static class Settings
{
    private static bool isBenchmark = false;

    public static void SetBenchmark()
    {
        isBenchmark = true;
    }

    public static bool IsBenchmark()
    {
        return isBenchmark;
    }
}
