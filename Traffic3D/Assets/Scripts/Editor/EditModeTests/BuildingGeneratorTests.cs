using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class BuildingGeneratorTests
{
    readonly string mapWithLargeNumBuildings = "Assets/Scripts/Editor/EditModeTests/MapFiles/LargeNumBuildings.txt";
    OpenStreetMapReader osmMapReader;
    int numBuildings;

    //import file and count number of buildings in file
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        osmMapReader = new OpenStreetMapReader();
        osmMapReader.ImportFile(mapWithLargeNumBuildings);

        numBuildings = 0;
        foreach (var way in osmMapReader.ways)
        {
            if (way.isBuilding)
                numBuildings++;
        }
    }

    [Test]
    public void CorrectNumBuildingsGeneratedFromWays()
    {
        BuildingGenerator buildingGenerator = new BuildingGenerator(osmMapReader, null);
        buildingGenerator.GenerateBuildings();

        Assert.True(buildingGenerator.buildingsCreated == numBuildings); //check number of roads == expected 
    }

    //100 roads every 10 Miliseconds
    [Test]
    public void TimeTakenToGenerateBuildings()
    {
        BuildingGenerator buildingGenerator = new BuildingGenerator(osmMapReader, null);
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        int maxTime = Mathf.CeilToInt((float)numBuildings / 100) * 10; //+10 miliseconds for every 100 buildings

        stopwatch.Start();
        buildingGenerator.GenerateBuildings();
        stopwatch.Stop();

        long timeTaken = stopwatch.ElapsedMilliseconds;
            
        Assert.True(timeTaken <= maxTime);
    }
}

