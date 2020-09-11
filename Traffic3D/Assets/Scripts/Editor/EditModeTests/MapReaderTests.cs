using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class MapReaderTests
{
    readonly string mapWithMaxNodes = "Assets/Scripts/Editor/EditModeTests/MapFiles/MaximumNodesDataSet.txt";
    readonly string mapFile = "Assets/Scripts/Editor/EditModeTests/MapFiles/SmallData.txt";

    //Tests if the system is storing the correct number of nodes
    [Test]
    public void StoreCorrectNumberOfNodes()
    {
        OpenStreetMapReader osmMapReader = new OpenStreetMapReader();
        osmMapReader.ImportFile(mapFile); // 
            
        Assert.True(osmMapReader.nodes.Count == 19831);
    }

    //Tests if the system is storing the correct number of ways
    [Test]
    public void StoreCorrectNumberOfWays()
    {
        OpenStreetMapReader osmMapReader = new OpenStreetMapReader();
        osmMapReader.ImportFile(mapFile);

        Assert.True(osmMapReader.ways.Count == 2760);
    }

    //Tests the time taken to read data from a large map file
    [Test]
    public void TimeToReadMaxSizeFile()
    {
        OpenStreetMapReader osmMapReader = new OpenStreetMapReader();         
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        stopwatch.Start();
        osmMapReader.ImportFile(mapWithMaxNodes);
        stopwatch.Stop();

        long timeTaken = stopwatch.ElapsedMilliseconds;

        Assert.True(timeTaken < 3000); //Shouldn't take more than 3 seconds when including inaccuracies of StopWatch
        Assert.True(osmMapReader.nodes.Count > 50000); //Ensure file has over 50K nodes
            
    }

}

