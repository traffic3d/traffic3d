using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MeshAssetTests
    {
        readonly string mapWithMaxNodes = Application.dataPath + "/Scripts/Editor/ImportOsmTests/Files/MaximumNodesDataSet.txt";
        readonly string mapWithLargeNumBuildings = Application.dataPath + "/Scripts/Editor/ImportOsmTests/Files/newYork.txt";

        [Test]
        public void TimeTakenToGenerateRoads()
        {
            MapReader mr = new MapReader();
            mr.ImportFile(mapWithMaxNodes);
            RoadGenerator rg = new RoadGenerator(mr, null, null);
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            stopwatch.Start();
            rg.GenerateRoads();
            stopwatch.Stop();

            long timeTaken = stopwatch.ElapsedMilliseconds;

            Assert.True(rg.GetWayObjects().Count == 655); //check number of roads == expected number
            Assert.True(timeTaken < 131); //100 roads should take less than 20 Miliseconds. => 20 * 6.55 = 131 MiliSeconds for 655 roads

        }

        [Test]
        public void TimeTakenToGenerateBuildings()
        {
            MapReader mr = new MapReader();
            mr.ImportFile(mapWithLargeNumBuildings);
            BuildingGenerator bg = new BuildingGenerator(mr, null);
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            stopwatch.Start();
            bg.GenerateBuildings();
            stopwatch.Stop();

            long timeTaken = stopwatch.ElapsedMilliseconds;

            Assert.True(bg.buildingsCreated == 1968); //check building count == expected number
            Assert.True(timeTaken < 200); //1000 buildings should take less than 100 Miliseconds to spawn. => 1968 buildings should take less than, approximately, 200ms to spawn. 

        }

    }
}
