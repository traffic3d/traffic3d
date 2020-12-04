using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class RoadGeneratorTests
{
    readonly string mapWithMaxNodes = "Assets/Scripts/Editor/EditModeTests/MapFiles/MaximumNodesDataSet.txt";
    OpenStreetMapReader osmMapReader;
    int numRoads;

    //import file and count number of roads in file
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        osmMapReader = new OpenStreetMapReader();
        osmMapReader.ImportFile(mapWithMaxNodes);

        numRoads = 0;
        foreach (var way in osmMapReader.ways)
        {
            if (way.IsRoad)
                numRoads++;
        }
    }

    [Test]
    public void CorrectNumRoadsGeneratedFromWays()
    {
        RoadGenerator roadGenerator = new RoadGenerator(osmMapReader, null);
        roadGenerator.GenerateRoads();
            
        Assert.True(roadGenerator.GetWayObjects().Count == numRoads); //check number of roads == expected 
    }
}

