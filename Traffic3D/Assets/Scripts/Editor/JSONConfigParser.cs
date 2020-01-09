using System;
using System.IO;
using UnityEngine;

public class JSONConfigParser
{
    public static void Parse(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Cannot find inputted JSON config file at: " + filePath);
        }
        string fileData = File.ReadAllText(filePath);
        JSONConfig config = JsonUtility.FromJson<JSONConfig>(fileData);

    }

    public class JSONConfig
    {
        public string testString;
    }
}
