using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class JunctionGeneratorTests
{
    readonly string mapFile = "Assets/Scripts/Editor/EditModeTests/MapFiles/SmallData.txt";

    OpenStreetMapReader osmMapReader;
    Dictionary<MapXmlWay, GameObject> origionalWayDic; //Not modified after [OneTimeSetUp]

    [SetUp]
    public void Setup()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        osmMapReader = new OpenStreetMapReader();
        origionalWayDic = new Dictionary<MapXmlWay, GameObject>();

        osmMapReader.ImportFile(mapFile);

        foreach (var way in osmMapReader.ways)
        {
            GameObject go = new GameObject(way.Name);
            origionalWayDic.Add(way, go);
        }

        GameObject vehicleFactoryGameObject = new GameObject();
        VehicleFactory vehicleFactory = vehicleFactoryGameObject.AddComponent<VehicleFactory>();
        var wayDicClone = new Dictionary<MapXmlWay, GameObject>(origionalWayDic);

        //run path generator 
        PathGenerator pathGenerator = new PathGenerator(osmMapReader, true);
        pathGenerator.AddPathsToRoads(wayDicClone);
    }
        
    /// <summary>
    /// Checks if junctions created at every mid-node (Cross-junction).
    /// </summary>
    [Test]
    public void CorrectNumJunctionsCreatedAtMidNodes()
    {
        JunctionGenerator junctionGenerator = new JunctionGenerator();
        junctionGenerator.GenerateJunctions(new TrafficLightGenerator(osmMapReader));

        Assert.AreEqual(junctionGenerator.GetNumCreatedJunctions(), (junctionGenerator.GetNumMidConnectedRoads()/2));
    }

    /// <summary>
    /// Ensure the number of start-nodes and end-nodes is the same as the total number of roads in the scene.
    /// Check before and after creating junctions.
    /// </summary>
    [Test]
    public void CorrectStartAndEndNodes()
    {
        JunctionGenerator junctionGenerator = new JunctionGenerator();

        //Before Junctions Created
        Assert.AreEqual(junctionGenerator.GetNumOfTotalEndNodes(), junctionGenerator.GetNumCreatedRoads());
        Assert.AreEqual(junctionGenerator.GetNumOfTotalStartNodes(), junctionGenerator.GetNumCreatedRoads());

        junctionGenerator.GenerateJunctions(new TrafficLightGenerator(osmMapReader));

        //After Junctions Created
        Assert.AreEqual(junctionGenerator.GetNumOfTotalEndNodes(), junctionGenerator.GetNumCreatedRoads());
        Assert.AreEqual(junctionGenerator.GetNumOfTotalStartNodes(), junctionGenerator.GetNumCreatedRoads());
    }


}

