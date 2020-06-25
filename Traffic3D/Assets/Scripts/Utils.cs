using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Utils
{
    private static string resultPath = System.IO.Path.Combine(Application.dataPath, "Results");

    public static void AppendAllTextToResults(string fileName, string result)
    {
        if (!Directory.Exists(resultPath))
        {
            Directory.CreateDirectory(resultPath);
        }
        System.IO.File.AppendAllText(System.IO.Path.Combine(resultPath, fileName), result);
    }

    public static string[] ReadText(string fileName)
    {
        return System.IO.File.ReadAllLines(System.IO.Path.Combine(resultPath, fileName));
    }
}
