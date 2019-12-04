using System;

public static class Settings
{
    private static bool isBenchmark = false;

    public static void SetBenchmark()
    {
        Console.WriteLine("Running Benchmark");
        isBenchmark = true;
    }

    public static bool IsBenchmark()
    {
        return isBenchmark;
    }
}
