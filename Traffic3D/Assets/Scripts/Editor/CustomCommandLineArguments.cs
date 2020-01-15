
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CustomCommandLineArguments
{
    public static void Run()
    {
        CheckHeadless();
        CheckJSONConfigFile();
        CheckOpenScene();
    }

    public static string GetArgument(string name)
    {
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Contains(name))
            {
                return args[i + 1];
            }
        }
        return null;
    }

    private static void CheckHeadless()
    {
        string isHeadless = GetArgument("RunHeadless");
        if (isHeadless != null && isHeadless.ToLower().Equals("true"))
        {
            Settings.SetHeadlessMode(true);
        }
        else
        {
            Settings.SetHeadlessMode(false);
        }
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

}
