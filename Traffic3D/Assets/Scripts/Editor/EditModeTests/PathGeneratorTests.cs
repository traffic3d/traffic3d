using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

[Category("Tests")]
public class PathGeneratorTests
{
    readonly string mapFile = "Assets/Scripts/Editor/EditModeTests/MapFiles/SmallData.txt";
    OpenStreetMapReader osmMapReader;
    private PathGenerator pathGenerator;

    int numRoads;

    //required by pathGenerator class
    VehicleFactory vehicleFactory;
    Dictionary<MapXmlWay, GameObject> wayDic;
    Dictionary<MapXmlWay, GameObject> defaultDic; // used to reset dictionary after each test 

    [SetUp]
    public void SetUp()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        numRoads = 0;

        wayDic = new Dictionary<MapXmlWay, GameObject>();

        osmMapReader = new OpenStreetMapReader();
        osmMapReader.ImportFile(mapFile);

        RoadGenerator roadGenerator = new RoadGenerator(osmMapReader, null);
        roadGenerator.GenerateRoads();
        pathGenerator = new PathGenerator(osmMapReader, true);
        pathGenerator.AddPathsToRoads(roadGenerator.GetWayObjects());
        RoadNetworkManager.GetInstance().Reload();
    }

    /// <summary>
    /// Ensure the number of start-nodes and end-nodes is the same as the total number of roads in the scene.
    /// </summary>
    [Test]
    public void CorrectStartAndEndNodes()
    {
        Assert.True(pathGenerator.GetNumOfTotalEndNodes() == pathGenerator.GetNumCreatedRoads());
        Assert.True(pathGenerator.GetNumOfTotalStartNodes() == pathGenerator.GetNumCreatedRoads());
    }

    /// <summary>
    /// Ensure correct number of lanes are generated
    /// </summary>
    [Test]
    public void CorrectNumberOfLanesGenerated()
    {
        List<Road> roadsToEvaluate = RoadNetworkManager.GetInstance().GetRoads();
        foreach (MapXmlWay way in osmMapReader.ways)
        {
            if (!way.IsRoad)
            {
                continue;
            }
            Road foundRoad = roadsToEvaluate.Find(r => r.name == way.Name && r.numberOfLanes == (way.ForwardLanes + way.BackwardLanes));
            Assert.IsNotNull(foundRoad, way.Name + " not generated.");
            roadsToEvaluate.Remove(foundRoad);
        }
        Assert.IsEmpty(roadsToEvaluate, "Unknown roads were generated.");
    }

}

