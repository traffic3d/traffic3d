﻿using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class JunctionGeneratorTest
    {
        readonly string mapFile = Application.dataPath + "/Scripts/Editor/ImportOsmTests/Files/smallData.txt";
        OpenStreetMapReader osmMapReader;
        Dictionary<MapXmlWay, GameObject> origionalWayDic; //Not modified after [OneTimeSetUp]

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            osmMapReader = new OpenStreetMapReader();
            origionalWayDic = new Dictionary<MapXmlWay, GameObject>();
            
            osmMapReader.ImportFile(mapFile);

            foreach (var way in osmMapReader.ways)
            {
                GameObject go = new GameObject(way.Name);
                origionalWayDic.Add(way, go);
            }
        }

        [SetUp]
        public void CreatePaths()
        {
            GameObject vehicleFactory = new GameObject();
            vehicleFactory.AddComponent<VehicleFactory>();
            var wayDicClone = new Dictionary<MapXmlWay, GameObject>(origionalWayDic);

            //run path generator 
            PathGenerator pathGenerator = new PathGenerator(osmMapReader, vehicleFactory);
            pathGenerator.AddPathsToRoads(wayDicClone);
            pathGenerator.JoinRoadsWithSameName();
            pathGenerator.PopulateVehicleFactory();
        }
        
        /// <summary>
        /// Checks if junctions created at every mid-node (Cross-junction).
        /// </summary>
        [Test]
        public void CorrectNumJunctionsCreatedAtMidNodes()
        {
            JunctionGenerator junctionGenerator = new JunctionGenerator();
            junctionGenerator.GenerateJunctions(new TrafficLightGenerator(osmMapReader));

            Assert.True(junctionGenerator.GetNumCreatedJunctions() == (junctionGenerator.GetNumMidConnectedRoads()/2));
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
            Assert.True(junctionGenerator.GetNumOfTotalEndNodes() == junctionGenerator.GetNumCreatedRoads());
            Assert.True(junctionGenerator.GetNumOfTotalStartNodes() == junctionGenerator.GetNumCreatedRoads());

            junctionGenerator.GenerateJunctions(new TrafficLightGenerator(osmMapReader));

            //After Junctions Created
            Assert.True(junctionGenerator.GetNumOfTotalEndNodes() == junctionGenerator.GetNumCreatedRoads());
            Assert.True(junctionGenerator.GetNumOfTotalStartNodes() == junctionGenerator.GetNumCreatedRoads());
        }


    }
}
