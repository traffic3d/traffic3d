
using System;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CustomCommandLineArguments
{
    private static string[] arguments = Environment.GetCommandLineArgs();

    public static void Run()
    {
        CheckHeadless();
        CheckJSONConfigFile();
        CheckOpenScene();
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
