using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class BuildingGeneratorTests
{
    readonly string mapWithLargeNumBuildings = "Assets/Scripts/Editor/EditModeTests/MapFiles/LargeNumBuildings.txt";
    OpenStreetMapReader osmMapReader;
    int numBuildings;

    //import file and count number of buildings in file
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        osmMapReader = new OpenStreetMapReader();
        osmMapReader.ImportFile(mapWithLargeNumBuildings);

        numBuildings = 0;
        foreach (var way in osmMapReader.ways)
        {
            if (way.isBuilding)
                numBuildings++;
        }
    }

    [Test]
    public void CorrectNumBuildingsGeneratedFromWays()
    {
        BuildingGenerator buildingGenerator = new BuildingGenerator(osmMapReader, null);
        buildingGenerator.GenerateBuildings();

        Assert.True(buildingGenerator.buildingsCreated == numBuildings); //check number of roads == expected 
    }
}

