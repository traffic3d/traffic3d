using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// GUI used to upload a .txt OpenStreetMap file, and asset textures
/// </summary>
public class ImportOsmGui : EditorWindow
{
    public string filePath = "Select an OSM file...";
    private Material road_material;
    private Material building_material;
    private Material floor_material;
    private float defaultLaneWidth = RoadGenerator.defaultLaneWidthStartValue;
    private bool isLeftHandDrive = true;

    private bool isValidFile;

    //true => secene already been imported. Must wait for new scene
    private bool isSceneActive;

    //Add to "Windows" tab
    [MenuItem("Traffic3D/Import OpenStreetMap File")]

    /// <summary>
    /// Create UI pop-up
    /// </summary>
    public static void DisplayOsmGui()
    {
        var window = GetWindow<ImportOsmGui>();
        window.titleContent = new GUIContent("Import OpenStreetMap file");
        window.Show();
    }

    /// <summary>
    /// Updates Values of progress bar
    /// </summary>
    /// <param name="progressValue">% progress is complete</param>
    /// <param name="progressText">Text displayed in progress bar</param>
    public void UpdateProgressBar(float progressValue, string progressText)
    {
        EditorUtility.DisplayProgressBar("Importing Map", string.Format("{0}", progressText, progressValue), progressValue);

        if (progressValue == 1f)
            EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// GUI Elements
    /// </summary>
    private void OnGUI()
    {
        //textbox disabled (users must use button and cannot directly type file-path)
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField(filePath);
        EditorGUI.EndDisabledGroup();

        // -- UI-Element: Button which asks For File....
        if (GUILayout.Button("Select File"))
        {
            var filePath = EditorUtility.OpenFilePanel("Select OSM File", Application.dataPath, "txt");
            isValidFile = false;

            if (filePath.Length > 0)
            {
                this.filePath = filePath;
                isValidFile = true;
            }
        }


        // -- UI-Element: Ask For Road Material
        road_material = EditorGUILayout.ObjectField("Select Road Material:", road_material, typeof(Material), false) as Material;

        // -- UI-Element: Ask For Floor Material
        floor_material = EditorGUILayout.ObjectField("Select Floor Material:", floor_material, typeof(Material), false) as Material;

        // -- UI-Element: Ask For Building Material
        building_material = EditorGUILayout.ObjectField("Select Building Material:", building_material, typeof(Material), false) as Material;

        // -- UI-Element: Ask For Default Lane Width
        defaultLaneWidth = EditorGUILayout.FloatField("Default Lane Width:", defaultLaneWidth);

        // -- UI-Element: Ask For Left Hand Drive
        isLeftHandDrive = EditorGUILayout.Toggle("Is Left Hand Drive:", isLeftHandDrive);

        // -- UI-Element: Import Button
        EditorGUI.BeginDisabledGroup(!isValidFile || isSceneActive);

        // -- UI-Element: Import button
        if (GUILayout.Button("Import Map File"))
        {
            //Import Map Data
            var osmSceneGenerator = new ImportOsmUiWrapper(this, filePath, road_material, floor_material, building_material, defaultLaneWidth, isLeftHandDrive);
            bool success = osmSceneGenerator.Import();

            if (!success)
            {
                EditorUtility.DisplayDialog("Error", "Error Uploading File", "Close");
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        EditorGUI.EndDisabledGroup();
    }

    private void Update()
    {
        isSceneActive = EditorSceneManager.GetActiveScene().isDirty;
    }
}
