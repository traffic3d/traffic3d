using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class Utils
{
    public static int WRITE_TIMEOUT = 1000;
    public static int READ_TIMEOUT = 1000;
    public static string FLOW_FILE_NAME = "Flow.csv";
    public static string THROUGHPUT_FILE_NAME = "Throughput.csv";
    public static string DENSITY_PER_KM_FILE_NAME = "DensityPerKm.csv";
    public static string VEHICLE_TIMES_FILE_NAME = "VehicleTimes.csv";
    public static string VEHICLE_DELAY_TIMES_FILE_NAME = "VehicleDelayTimes.csv";
    private static Dictionary<string, ReaderWriterLock> readerWriterLocks = new Dictionary<string, ReaderWriterLock>();
    private readonly static object lockObject = new object();

    private static string resultPath = System.IO.Path.Combine(Application.dataPath, "Results");

    public static void AppendAllTextToResults(string fileName, string result)
    {
        if (!Directory.Exists(resultPath))
        {
            Directory.CreateDirectory(resultPath);
        }
        ReaderWriterLock readerWriterLock = GetReaderWriterLockForFile(fileName);
        try
        {
            readerWriterLock.AcquireWriterLock(WRITE_TIMEOUT);
            System.IO.File.AppendAllText(System.IO.Path.Combine(resultPath, fileName), result);
        }
        finally
        {
            readerWriterLock.ReleaseWriterLock();
        }
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
        ReaderWriterLock readerWriterLock = GetReaderWriterLockForFile(fileName);
        try
        {
            readerWriterLock.AcquireReaderLock(READ_TIMEOUT);
            return System.IO.File.ReadAllLines(filePath);
        }
        finally
        {
            readerWriterLock.ReleaseReaderLock();
        }
    }

    public static bool IsTruthy(string value)
    {
        return value == "yes" || value == "true" || value == "1";
    }

    private static ReaderWriterLock GetReaderWriterLockForFile(string fileName)
    {
        lock (lockObject)
        {
            if (readerWriterLocks.ContainsKey(fileName))
            {
                return readerWriterLocks[fileName];
            }
            else
            {
                ReaderWriterLock readerWriterLock = new ReaderWriterLock();
                readerWriterLocks[fileName] = readerWriterLock;
                return readerWriterLock;
            }
        }
    }
}
