using System;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CustomCommandLineArguments
{
    private static string[] arguments = Environment.GetCommandLineArgs();

    public static void Run()
    {
        CheckJSONConfigFile();
        CheckRunBenchmark();
        CheckOpenScene();
        CheckRandomSeed();
    }

    public static string GetArgument(string name)
    {
        for (int i = 0; i < arguments.Length; i++)
        {
            if (arguments[i].Contains(name))
            {
                return arguments[i + 1];
            }
        }
        return null;
    }

    public static void SetMockArgument(string[] mockArguments)
    {
        arguments = mockArguments;
    }

    private static void CheckJSONConfigFile()
    {
        string configPath = GetArgument("JSONConfigFile");
        if (configPath != null)
        {
            JSONConfigParser.Parse(configPath);
        }
    }

    private static void CheckOpenScene()
    {
        string openScene = GetArgument("OpenScene");
        if (openScene != null)
        {
            EditorSceneManager.OpenScene(System.IO.Path.Combine(Application.dataPath, openScene));
        }
    }


    private static void CheckRunBenchmark()
    {
        string runBenchmark = GetArgument("RunBenchmark");
        if (runBenchmark != null && runBenchmark.ToLower().Equals("true"))
        {
            Settings.SetBenchmark();
            UnityEditor.EditorApplication.isPlaying = true;
            if (EditorSceneManager.GetActiveScene() == null)
            {
                EditorSceneManager.LoadScene("DayDemo");
            }
        }
    }

    private static void CheckRandomSeed()
    {
        string randomSeedString = GetArgument("RandomSeed");
        if (randomSeedString == null)
        {
            return;
        }
        if (int.TryParse(randomSeedString, out int randomSeed))
        {
            Settings.SetRandomSeed(randomSeed);
            RandomNumberGenerator.ReloadInstance();
        }
        else
        {
            throw new ArgumentException("RandomSeed must have an integer as a value.");
        }
    }

}
