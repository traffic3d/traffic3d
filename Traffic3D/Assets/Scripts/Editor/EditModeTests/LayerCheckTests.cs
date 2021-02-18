using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class LayerCheckTests
{
    readonly string mapFile = "Assets/Scripts/Editor/EditModeTests/MapFiles/SmallData.txt";
    ImportOsmUiWrapper osmWrapper;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        osmWrapper = new ImportOsmUiWrapper(null, mapFile, null, null, null, RoadGenerator.defaultLaneWidthStartValue, true);
        osmWrapper.Import();
    }

    [Test]
    public void CheckIgnoreRayCastLayerNumber()
    {
        Assert.True(LayerMask.LayerToName(2) == "Ignore Raycast");
    }

    //Check existing trafficlights to ensure they have 'ignore raycast' layer
    [Test]
    public void TrafficLightIgnoreRayCast()
    {
        foreach (GameObject gameObj in GameObject.FindObjectsOfType<GameObject>())
        {
            //Check all currently existing trafficlight names
            if (gameObj.name == "TrafficLight_1" || gameObj.name == "TrafficLight_2")
            {
                Assert.True(gameObj.layer == LayerMask.NameToLayer("Ignore Raycast"));
            }
        }
    }

    //Check existing vehicle Paths to ensure they have 'ignore raycast' layer
    [Test]
    public void RoadWaysIgnoreRayCast()
    {
        //get all objects with Path script
        RoadWay[] roadWays = GameObject.FindObjectsOfType<RoadWay>();

        //ensure all paths have 'ignore raycast' layer
        foreach (RoadWay roadWay in roadWays)
        {
            Assert.True(roadWay.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"));
        }
    }

    //Check existing Junctions to ensure they have 'ignore raycast' layer
    [Test]
    public void JunctionsIgnoreRayCast()
    {
        //get all objects with Junction script
        Junction[] junctions = (Junction[])GameObject.FindObjectsOfType(typeof(Junction));

        //ensure all junctions have 'ignore raycast' layer
        foreach (Junction junction in junctions)
        {
            Assert.True(junction.gameObject.layer == 2);
        }
    }


}

