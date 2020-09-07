using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class BuildingGeneratorTest
    {
        readonly string mapWithLargeNumBuildings = Application.dataPath + "/Scripts/Editor/ImportOsmTests/Files/newYork.txt";
        MapReader mapReader;
        int numBuildings;

        //import file and count number of buildings in file
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            mapReader = new MapReader();
            mapReader.ImportFile(mapWithLargeNumBuildings);

            numBuildings = 0;
            foreach (var way in mapReader.ways)
            {
                if (way.isBuilding)
                    numBuildings++;
            }
        }

        [Test]
        public void CorrectNumBuildingsGeneratedFromWays()
        {
            BuildingGenerator buildingGenerator = new BuildingGenerator(mapReader, null);
            buildingGenerator.GenerateBuildings();

            Assert.True(buildingGenerator.buildingsCreated == numBuildings); //check number of roads == expected 
        }

        //100 roads every 10 Miliseconds
        [Test]
        public void TimeTakenToGenerateBuildings()
        {
            BuildingGenerator buildingGenerator = new BuildingGenerator(mapReader, null);
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            int maxTime = Mathf.CeilToInt((float)numBuildings / 100) * 10; //+10 miliseconds for every 100 buildings

            stopwatch.Start();
            buildingGenerator.GenerateBuildings();
            stopwatch.Stop();

            long timeTaken = stopwatch.ElapsedMilliseconds;
            
            Assert.True(timeTaken <= maxTime);
        }
    }
}
