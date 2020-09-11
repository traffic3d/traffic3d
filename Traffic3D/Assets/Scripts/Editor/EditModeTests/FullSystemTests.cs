using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class FullSystemTests
{
    readonly string mapWithMaxNodes = "Assets/Scripts/Editor/EditModeTests/MapFiles/MaximumNodesDataSet.txt";


    // Test how quickly the system as a whole can import the largest map file
    [Test]
    public void MaxFileLoadTimes()
    {
        ImportOsmUiWrapper handler = new ImportOsmUiWrapper(null, mapWithMaxNodes,null,null,null);

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        stopwatch.Start();
        handler.Import();
        stopwatch.Stop();
            
        Assert.True(handler.GetNodesInScene() >= 50000); // Ensure file has over 50,000 nodes
        Assert.True(stopwatch.ElapsedMilliseconds < 5000); // >5 Seconds

    }
        
}

