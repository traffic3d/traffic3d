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

    public static bool GetLineIntersection(Vector2 line1p1, Vector2 line1p2, Vector2 line2p1, Vector2 line2p2, out Vector2 intersection)
    {
        intersection = Vector2.zero;
        float d = (line1p2.x - line1p1.x) * (line2p2.y - line2p1.y) - (line1p2.y - line1p1.y) * (line2p2.x - line2p1.x);
        if (d == 0.0f)
        {
            return false;
        }
        float u = ((line2p1.x - line1p1.x) * (line2p2.y - line2p1.y) - (line2p1.y - line1p1.y) * (line2p2.x - line2p1.x)) / d;
        float v = ((line2p1.x - line1p1.x) * (line1p2.y - line1p1.y) - (line2p1.y - line1p1.y) * (line1p2.x - line1p1.x)) / d;
        if (u <= 0.0f || u >= 1.0f || v <= 0.0f || v >= 1.0f)
        {
            return false;
        }
        intersection.x = line1p1.x + u * (line1p2.x - line1p1.x);
        intersection.y = line1p1.y + u * (line1p2.y - line1p1.y);
        return true;
    }

    private static ReaderWriterLock GetReaderWriterLockForFile(string fileName)
    {
        lock (lockObject) {
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
