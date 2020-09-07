using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class RoadGeneratorTest
    {
        readonly string mapWithMaxNodes = Application.dataPath + "/Scripts/Editor/ImportOsmTests/Files/MaximumNodesDataSet.txt";
        MapReader mapReader;
        int numRoads;

        //import file and count number of roads in file
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            mapReader = new MapReader();
            mapReader.ImportFile(mapWithMaxNodes);

            numRoads = 0;
            foreach (var way in mapReader.ways)
            {
                if (way.IsRoad)
                    numRoads++;
            }
        }

        [Test]
        public void CorrectNumRoadsGeneratedFromWays()
        {
            RoadGenerator roadGenerator = new RoadGenerator(mapReader, null);
            roadGenerator.GenerateRoads();
            
            Assert.True(roadGenerator.GetWayObjects().Count == numRoads); //check number of roads == expected 
        }

        //100 roads every 20 Miliseconds
        [Test]
        public void TimeTakenToGenerateRoads()
        {
            RoadGenerator roadGenerator = new RoadGenerator(mapReader, null);
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
         
            int maxTime = Mathf.CeilToInt((float)numRoads / 100) * 20; //+20 miliseconds for every 100 roads

            stopwatch.Start();
            roadGenerator.GenerateRoads();
            stopwatch.Stop();

            long timeTaken = stopwatch.ElapsedMilliseconds;
            
            Assert.True(timeTaken <= maxTime);
        }
    }
}
