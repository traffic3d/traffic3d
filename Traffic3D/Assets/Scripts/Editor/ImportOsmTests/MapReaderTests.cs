using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MapReaderTests
    {
        readonly string mapWithMaxNodes = Application.dataPath + "/Scripts/Editor/ImportOsmTests/Files/MaximumNodesDataSet.txt";
        readonly string mapFile = Application.dataPath + "/Scripts/Editor/ImportOsmTests/Files/SmallData.txt";

        //Tests if the system is storing the correct number of nodes
        [Test]
        public void StoreCorrectNumberOfNodes()
        {
            MapReader mr = new MapReader();
            mr.ImportFile(mapFile); // 
            
            Assert.True(mr.nodes.Count == 19831);
        }

        //Tests if the system is storing the correct number of ways
        [Test]
        public void StoreCorrectNumberOfWays()
        {
            MapReader mr = new MapReader();
            mr.ImportFile(mapFile);

            Assert.True(mr.ways.Count == 2760);
        }

        //Tests the time taken to read data from a large map file
        [Test]
        public void TimeToReadMaxSizeFile()
        {
            MapReader mr = new MapReader();         
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            stopwatch.Start();
            mr.ImportFile(mapWithMaxNodes);
            stopwatch.Stop();

            long timeTaken = stopwatch.ElapsedMilliseconds;

            Assert.True(timeTaken < 3000); //Shouldn't take more than 3 seconds when including inaccuracies of StopWatch
            Assert.True(mr.nodes.Count > 50000); //Ensure file has over 50K nodes
            
        }

    }
}
