﻿using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

[Category("Tests")]
public class PathGeneratorTests
{
    readonly string mapFile = "Assets/Scripts/Editor/EditModeTests/MapFiles/SmallData.txt";
    OpenStreetMapReader osmMapReader;

    int numRoads;

    //required by pathGenerator class
    VehicleFactory vehicleFactory;
    Dictionary<MapXmlWay, GameObject> wayDic;
    Dictionary<MapXmlWay, GameObject> defaultDic; // used to reset dictionary after each test 

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        numRoads = 0;

        wayDic = new Dictionary<MapXmlWay, GameObject>();

        osmMapReader = new OpenStreetMapReader();
        osmMapReader.ImportFile(mapFile);

        foreach (var way in osmMapReader.ways)
        {
            if (way.IsRoad)
                numRoads++;

            GameObject go = new GameObject(way.Name);
            wayDic.Add(way, go);
        }

        defaultDic = new Dictionary<MapXmlWay, GameObject>(wayDic); //clone
    }

    //reset dictionary after each test
    [TearDown]
    public void ResetDictionary()
    {
        wayDic = new Dictionary<MapXmlWay, GameObject>(defaultDic); //clone origional
    }

    //check if number of paths created and added to vehicle factory is correct (before any roads are merged)
    [Test]
    public void CorrectNumRoadWaysGeneratedFromWays()
    {
        PathGenerator pathGenerator = new PathGenerator(osmMapReader);
        pathGenerator.AddPathsToRoads(wayDic);
        RoadNetworkManager.GetInstance().Reload();

        Assert.AreEqual(numRoads, RoadNetworkManager.GetInstance().GetWays().Count);
    }

    /// <summary>
    /// Ensure the number of start-nodes and end-nodes is the same as the total number of roads in the scene.
    /// </summary>
    [Test]
    public void CorrectStartAndEndNodes()
    {
        //Before merging roads
        PathGenerator pathGenerator = new PathGenerator(osmMapReader);
        pathGenerator.AddPathsToRoads(wayDic);

        Assert.True(pathGenerator.GetNumOfTotalEndNodes() == pathGenerator.GetNumCreatedRoads());
        Assert.True(pathGenerator.GetNumOfTotalStartNodes() == pathGenerator.GetNumCreatedRoads());
    }

}

