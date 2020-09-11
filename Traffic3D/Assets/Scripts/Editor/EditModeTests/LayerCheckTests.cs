using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class LayerCheckTests
{
    readonly string mapFile = "Assets/Scripts/Editor/EditModeTests/MapFiles/SmallData.txt";
    ImportOsmUiWrapper osmWrapper;

    [Test]
    public void CheckIgnoreRayCastLayerNumber()
    {
        Assert.True(LayerMask.LayerToName(2) == "Ignore Raycast");
    }

    //Check existing trafficlights to ensure they have 'ignore raycast' layer
    [Test]
    public void TrafficLightIgnoreRayCast()
    {
        osmWrapper = new ImportOsmUiWrapper(null, mapFile, null, null, null);
        osmWrapper.Import();
            
        foreach (GameObject gameObj in GameObject.FindObjectsOfType<GameObject>())
        {
            //Check all currently existing trafficlight names
            if (gameObj.name == "TrafficLight_1" || gameObj.name == "TrafficLight_2")
            {
                Assert.True(gameObj.layer == 2);
            }
        }
    }

    //Check existing vehicle Paths to ensure they have 'ignore raycast' layer
    [Test]
    public void PathsIgnoreRayCast()
    {
        osmWrapper = new ImportOsmUiWrapper(null, mapFile, null, null, null);
        osmWrapper.Import();

        //get all objects with Path script
        Path[] paths = (Path[])GameObject.FindObjectsOfType(typeof(Path));

        //ensure all paths have 'ignore raycast' layer
        foreach (Path path in paths)
        {
            Assert.True(path.gameObject.layer == 2);
        }
    }

    //Check existing Junctions to ensure they have 'ignore raycast' layer
    [Test]
    public void JunctionsIgnoreRayCast()
    {
        osmWrapper = new ImportOsmUiWrapper(null, mapFile, null, null, null);
        osmWrapper.Import();

        //get all objects with Junction script
        Junction[] junctions = (Junction[])GameObject.FindObjectsOfType(typeof(Junction));

        //ensure all junctions have 'ignore raycast' layer
        foreach (Junction junction in junctions)
        {
            Assert.True(junction.gameObject.layer == 2);
        }
    }


}

