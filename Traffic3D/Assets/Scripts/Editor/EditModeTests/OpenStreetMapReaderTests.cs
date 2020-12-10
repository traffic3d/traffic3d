using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class OpenStreetMapReaderTests
{
    readonly string mapWithMaxNodes = "Assets/Scripts/Editor/EditModeTests/MapFiles/MaximumNodesDataSet.txt";
    readonly string mapFile = "Assets/Scripts/Editor/EditModeTests/MapFiles/SmallData.txt";

    [OneTimeSetUp]
    public void SetUp()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    }

    //Tests if the system is storing the correct number of nodes
    [Test]
    public void StoreCorrectNumberOfNodes()
    {
        OpenStreetMapReader osmMapReader = new OpenStreetMapReader();
        osmMapReader.ImportFile(mapFile); // 
            
        Assert.True(osmMapReader.nodes.Count == 19831);
    }

    //Tests if the system is storing the correct number of ways
    [Test]
    public void StoreCorrectNumberOfWays()
    {
        OpenStreetMapReader osmMapReader = new OpenStreetMapReader();
        osmMapReader.ImportFile(mapFile);

        Assert.True(osmMapReader.ways.Count == 2760);
    }

}

