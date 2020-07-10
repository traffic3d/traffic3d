using System.IO;
using UnityEngine;

public class Utils
{
    public static string FLOW_FILE_NAME = "Flow.csv";
    public static string THROUGHPUT_FILE_NAME = "Throughput.csv";
    public static string DENSITY_PER_KM_FILE_NAME = "DensityPerKm.csv";
    public static string VEHICLE_TIMES_FILE_NAME = "VehicleTimes.csv";
    public static string VEHICLE_DELAY_TIMES_FILE_NAME = "VehicleDelayTimes.csv";

    private static string resultPath = System.IO.Path.Combine(Application.dataPath, "Results");

    public static void AppendAllTextToResults(string fileName, string result)
    {
        if (!Directory.Exists(resultPath))
        {
            Directory.CreateDirectory(resultPath);
        }
        System.IO.File.AppendAllText(System.IO.Path.Combine(resultPath, fileName), result);
    }

    public static string[] ReadResultText(string fileName)
    {
        if (!Directory.Exists(resultPath))
        {
            return null;
        }
        string filePath = System.IO.Path.Combine(resultPath, fileName);
        if (!File.Exists(filePath))
        {
            return null;
        }
        return System.IO.File.ReadAllLines(filePath);
    }
}
