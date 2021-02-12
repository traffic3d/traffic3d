using System.Collections;
using NUnit.Framework;
using Tests;
using UnityEngine;
using UnityEngine.TestTools;


[Category("Tests")]
public class EvacuAgentPedestrianFactoryTests : EvacuAgentCommonSceneTest
{
    [UnityTest]
    public IEnumerator PedestrianFactory_WillSpawnPedestrian_WithIsUsingEvacuationBehaviourTrue_WhenEvacuAgentSceneIsActive()
    {
        // Arrange
        DisableLoops();
        yield return null;
        foreach (Pedestrian pedestrian in GameObject.FindObjectsOfType<Pedestrian>())
        {
            GameObject.Destroy(pedestrian);
        }
        Assert.Zero(GameObject.FindObjectsOfType<Pedestrian>().Length);
        PedestrianFactory pedestrianFactory = (PedestrianFactory)GameObject.FindObjectOfType(typeof(PedestrianFactory));

        // Act
        pedestrianFactory.SpawnPedestrian();
        Pedestrian pedestrianTwo = GameObject.FindObjectOfType<Pedestrian>();

        // Assert
        Assert.IsTrue(pedestrianTwo.isUsingEvacuationBehaviour);
    }
}
