using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PathGeneratorTest
    {
        readonly string mapFile = Application.dataPath + "/Scripts/Editor/ImportOsmTests/Files/SmallData.txt";
        OpenStreetMapReader osmMapReader;

        int numRoads;
        
        //required by pathGenerator class
        GameObject vehicleFactory;
        Dictionary<MapXmlWay, GameObject> wayDic;
        Dictionary<MapXmlWay, GameObject> defaultDic; // used to reset dictionary after each test 

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            numRoads = 0;

            vehicleFactory = new GameObject();
            vehicleFactory.AddComponent<VehicleFactory>();

            wayDic = new Dictionary<MapXmlWay, GameObject>();

            osmMapReader = new OpenStreetMapReader();
            osmMapReader.ImportFile(mapFile);

            foreach (var way in osmMapReader.ways)
            {                
                if (way.IsRoad)
                    numRoads++;
                    
                GameObject go = new GameObject(way.Name);
                wayDic.Add(way,go);
            }

            defaultDic = new Dictionary<MapXmlWay, GameObject>(wayDic); //clone
        }

        //reset vehicle factory after each test
        [TearDown]
        public void ResetVehicleFactory()
        {
            Object.DestroyImmediate(vehicleFactory);
            vehicleFactory = new GameObject();
            vehicleFactory.AddComponent<VehicleFactory>();
        }

        //reset dictionary after each test
        [TearDown]
        public void ResetDictionary()
        {
            wayDic = new Dictionary<MapXmlWay, GameObject>(defaultDic); //clone origional
        }

        //check if number of paths created and added to vehicle factory is correct (before any roads are merged)
        [Test]
        public void CorrectNumPathsGeneratedFromWays()
        {
            PathGenerator pathGenerator = new PathGenerator(osmMapReader, vehicleFactory);
            pathGenerator.AddPathsToRoads(wayDic);

            Assert.True(vehicleFactory.GetComponent<VehicleFactory>().paths.Count == numRoads);
        }


        /// <summary>
        /// Check if roads with the same name, travelling in the same direction, connected at the same point, are merged together into a single road
        /// Manually checked: for SmallData.txt there should initially be 102 roads and after merging, 64 roads should remain.
        /// </summary>
        [Test]
        public void MergeRoadsWithSimilarNamesTest()
        {
            int numRoadsBeforeMerging = 102;
            int numRoadsAfterMerging = 64;

            PathGenerator pathGenerator = new PathGenerator(osmMapReader, vehicleFactory);
            pathGenerator.AddPathsToRoads(wayDic);

            Assert.True(vehicleFactory.GetComponent<VehicleFactory>().paths.Count == numRoadsBeforeMerging);
            Assert.True(pathGenerator.GetNumCreatedRoads() == numRoadsBeforeMerging);

            //Check roads correctly merged
            pathGenerator.JoinRoadsWithSameName();
            pathGenerator.PopulateVehicleFactory(); // update vehicle factory

            Assert.True(vehicleFactory.GetComponent<VehicleFactory>().paths.Count == numRoadsAfterMerging);
            Assert.True(pathGenerator.GetNumCreatedRoads() == numRoadsAfterMerging);
        }

        /// <summary>
        /// Ensure the number of start-nodes and end-nodes is the same as the total number of roads in the scene.
        /// </summary>
        [Test]
        public void CorrectStartAndEndNodes()
        {
            //Before merging roads
            PathGenerator pathGenerator = new PathGenerator(osmMapReader, vehicleFactory);
            pathGenerator.AddPathsToRoads(wayDic);

            Assert.True(pathGenerator.GetNumOfTotalEndNodes() == pathGenerator.GetNumCreatedRoads());
            Assert.True(pathGenerator.GetNumOfTotalStartNodes() == pathGenerator.GetNumCreatedRoads());

            //After merging roads
            pathGenerator.JoinRoadsWithSameName();
            pathGenerator.PopulateVehicleFactory();

            Assert.True(pathGenerator.GetNumOfTotalEndNodes() == pathGenerator.GetNumCreatedRoads());
            Assert.True(pathGenerator.GetNumOfTotalStartNodes() == pathGenerator.GetNumCreatedRoads());
        }

    }
}
