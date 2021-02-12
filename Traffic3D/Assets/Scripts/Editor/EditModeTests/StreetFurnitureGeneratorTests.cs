using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;

[Category("Tests")]
public class StreetFurnitureGeneratorTests
{
    readonly string mapFile = "Assets/Scripts/Editor/EditModeTests/MapFiles/SmallData.txt";
    readonly int numOfAmenities = 77;
    readonly int numOfEmergency = 10;
    readonly int numOfPostBoxes = 6;
    readonly int numOfFireHydrant = 10;
    OpenStreetMapReader osmMapReader;

    //import file and count number of roads in file
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        osmMapReader = new OpenStreetMapReader();
        osmMapReader.ImportFile(mapFile);
    }

    [Test]
    public void CorrectNumAmenitiesAndEmergencyTest()
    {
        StreetFurnitureGenerator streetFurnitureGenerator = new StreetFurnitureGenerator(osmMapReader, true);
        streetFurnitureGenerator.GenerateStreetFurniture();
        int currentPostBoxes = 0;
        int currentFireHydrant = 0;
        foreach(Transform child in streetFurnitureGenerator.GetRootGameObject().transform)
        {
            if (child.name.StartsWith(OpenStreetMapTagName.postBoxItemName))
            {
                currentPostBoxes++;
            }
            if (child.name.StartsWith(OpenStreetMapTagName.fireHydrantItemName))
            {
                currentFireHydrant++;
            }
        }
        Assert.AreEqual(numOfPostBoxes, currentPostBoxes);
        Assert.AreEqual(numOfFireHydrant, currentFireHydrant);
    }
}
