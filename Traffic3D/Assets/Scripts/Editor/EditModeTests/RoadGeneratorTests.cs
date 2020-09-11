using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class RoadGeneratorTests
{
    readonly string mapWithMaxNodes = "Assets/Scripts/Editor/EditModeTests/MapFiles/MaximumNodesDataSet.txt";
    OpenStreetMapReader osmMapReader;
    int numRoads;

    //import file and count number of roads in file
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        osmMapReader = new OpenStreetMapReader();
        osmMapReader.ImportFile(mapWithMaxNodes);

        numRoads = 0;
        foreach (var way in osmMapReader.ways)
        {
            if (way.IsRoad)
                numRoads++;
        }
    }

    [Test]
    public void CorrectNumRoadsGeneratedFromWays()
    {
        RoadGenerator roadGenerator = new RoadGenerator(osmMapReader, null);
        roadGenerator.GenerateRoads();
            
        Assert.True(roadGenerator.GetWayObjects().Count == numRoads); //check number of roads == expected 
    }

    //100 roads every 20 Miliseconds
    [Test]
    public void TimeTakenToGenerateRoads()
    {
        RoadGenerator roadGenerator = new RoadGenerator(osmMapReader, null);
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
         
        int maxTime = Mathf.CeilToInt((float)numRoads / 100) * 20; //+20 miliseconds for every 100 roads

        stopwatch.Start();
        roadGenerator.GenerateRoads();
        stopwatch.Stop();

        long timeTaken = stopwatch.ElapsedMilliseconds;
            
        Assert.True(timeTaken <= maxTime);
    }
}

