using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SimulationControllerUI : EditorWindow
{
    private GUIStyle headStyle;
    private List<Type> displayedOptions = new List<Type>();

    //Add to "Windows" tab
    [MenuItem("Traffic3D/Simulation Controller")]

    /// <summary>
    /// Create UI pop-up
    /// </summary>
    public static SimulationControllerUI DisplayOsmGui()
    {
        SimulationControllerUI window = GetWindow<SimulationControllerUI>();
        window.titleContent = new GUIContent("Simulation Controller");
        window.Show();
        return window;
    }

    private void Awake()
    {
        headStyle = new GUIStyle();
        headStyle.alignment = TextAnchor.MiddleCenter;
    }

    public List<Type> GetDisplayedOptions()
    {
        return displayedOptions;
    }

    /// <summary>
    /// GUI Elements
    /// </summary>
    private void OnGUI()
    {
        displayedOptions.Clear();
        GUILayout.Label("Current Scene", headStyle);
        // -- UI-Element: Button to load a scene
        if (GUILayout.Button(new GUIContent(EditorSceneManager.GetActiveScene().name, "Click to load another scene.")))
        {
            var filePath = EditorUtility.OpenFilePanel("Load a Scene", System.IO.Path.Combine(Application.dataPath, "Scenes"), "unity");

            if (filePath.Length > 0)
            {
                EditorSceneManager.OpenScene(filePath);
            }
        }
        DisplaySimulationControls();
        GUILayout.Label("Options", headStyle);
        if (EditorApplication.isPlaying)
        {
            GUIStyle warningStyle = new GUIStyle(headStyle);
            warningStyle.normal.textColor = Color.red;
            warningStyle.fontStyle = FontStyle.Bold;
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextArea("Warning" + Environment.NewLine + "Option changes do not get saved" + Environment.NewLine + " when a simulation is playing.", warningStyle);
            EditorGUI.EndDisabledGroup();
        }
        List<Type> optionList = new List<Type>();
        optionList.Add(typeof(VehicleFactory));
        optionList.Add(typeof(PedestrianFactory));
        optionList.Add(typeof(EnvironmentSettings));
        optionList.Add(typeof(GraphManager));
        foreach (Type optionType in optionList)
        {
            DisplayGameObject(optionType);
        }
        if (GameObject.FindObjectOfType<SumoManager>() != null)
        {
            DisplayGameObject(typeof(SumoManager));
        }
        DisplayMiscellaneous();
    }

    private void Update()
    {

    }

    private void DisplaySimulationControls()
    {
        GUILayout.Label("Simulation Controls", headStyle);

        GUILayout.BeginHorizontal();
        // -- UI-Element: Play simulation
        EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying && !EditorApplication.isPaused);
        if (GUILayout.Button(new GUIContent("Play", "Start the simulation.")))
        {
            EditorApplication.isPlaying = true;
            if (EditorApplication.isPaused)
            {
                EditorApplication.isPaused = false;
            }
        }
        EditorGUI.EndDisabledGroup();

        // -- UI-Element: Pause simulation
        EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying || EditorApplication.isPaused);
        if (GUILayout.Button(new GUIContent("Pause", "Pause the simulation.")))
        {
            EditorApplication.isPaused = true;
        }
        EditorGUI.EndDisabledGroup();

        // -- UI-Element: Stop simulation
        EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
        if (GUILayout.Button(new GUIContent("Stop", "End the simulation.")))
        {
            EditorApplication.isPlaying = false;
            EditorApplication.isPaused = false;
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();
    }

    private void DisplayGameObject(Type type)
    {
        Component component = (Component)GameObject.FindObjectOfType(type);
        string buttonName;
        if (component == null)
        {
            buttonName = type.Name + " Doesn't Exist";
        }
        else
        {
            buttonName = "Open " + type.Name + " Options";
            displayedOptions.Add(type);
        }
        EditorGUI.BeginDisabledGroup(component == null);
        if (GUILayout.Button(new GUIContent(buttonName, GetDescription(type))))
        {
            Selection.activeGameObject = component.gameObject;
        }
        EditorGUI.EndDisabledGroup();
    }

    private void DisplayMiscellaneous()
    {
        GUILayout.Label("Miscellaneous", headStyle);

        // -- UI-Element: Ask for seed
        int seed = RandomNumberGenerator.GetSeed();
        int previousSeed = seed;
        seed = EditorGUILayout.IntField("Random Seed:", seed);
        if (seed != previousSeed)
        {
            Settings.SetRandomSeed(seed);
            RandomNumberGenerator.ReloadInstance();
        }
        // -- UI-Element: Ask for capture rate
        PythonManager pythonManager = GameObject.FindObjectOfType<PythonManager>();
        int captureRate = 0;
        if (pythonManager != null)
        {
            captureRate = pythonManager.frameRate;
        }
        EditorGUI.BeginDisabledGroup(pythonManager == null);
        captureRate = EditorGUILayout.IntField("Capture Rate:", captureRate);
        EditorGUI.EndDisabledGroup();
        if (pythonManager != null)
        {
            pythonManager.frameRate = captureRate;
        }
    }

    private string GetDescription(Type type)
    {
        if (type == typeof(VehicleFactory))
        {
            return "Change vehicle spawn rates, probablities of different vehicles and other vehicle related properties. Also add new vehicles.";
        }
        else if (type == typeof(PedestrianManager))
        {
            return "Edit pedestrian spawn rates, probability of different pedestrians and the maximum amount of pedestrians. Also add new pedestrian models.";
        }
        else if (type == typeof(EnvironmentSettings))
        {
            return "Change environment variables such as weather, lighting settings, skyboxes and surface materials.";
        }
        else if (type == typeof(GraphManager))
        {
            return "Select which graphs are displayed in real-time on screen.";
        }
        else if (type == typeof(SumoManager))
        {
            return "The overview of the SUMO interface. Change the IP, port and which components SUMO controls within the simulation.";
        }
        else
        {
            return "";
        }
    }

}
