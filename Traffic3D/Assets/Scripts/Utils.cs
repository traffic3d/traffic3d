using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    private static string resultPath = System.IO.Path.Combine("Assets", "Results");

    public static void AppendAllTextToResults(string fileName, string result)
    {
        System.IO.File.AppendAllText(System.IO.Path.Combine(resultPath, fileName), result);
    }
}
