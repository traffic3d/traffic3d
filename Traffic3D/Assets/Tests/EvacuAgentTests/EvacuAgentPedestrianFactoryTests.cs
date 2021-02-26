using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PedestrianFactory_WillSpawnPedestrian_WithIsUsingEvacuationBehaviourTrue_WhenEvacuAgentSceneIsActive : ArrangeActAssertStrategy
{
    private PedestrianFactory pedestrianFactory;
    private Pedestrian pedestrianTwo;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        yield return null;
        Arrange();
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        DisableLoops();

        foreach (Pedestrian pedestrian in GameObject.FindObjectsOfType<Pedestrian>())
        {
            GameObject.Destroy(pedestrian);
        }

        Assert.Zero(GameObject.FindObjectsOfType<Pedestrian>().Length);
        pedestrianFactory = (PedestrianFactory)GameObject.FindObjectOfType(typeof(PedestrianFactory));
    }

    public override void Act()
    {
        pedestrianFactory.SpawnPedestrian();
        pedestrianTwo = GameObject.FindObjectOfType<Pedestrian>();
    }

    public override void Assertion()
    {
        Assert.IsTrue(pedestrianTwo.isUsingEvacuationBehaviour);
    }
}

public class PedestrianFactory_WillSpawnPedestrian_withFieldOfView_WhenIsUsingEvacuationBehaviourIsTrue : ArrangeActAssertStrategy
{
    private PedestrianFactory pedestrianFactory;
    private Pedestrian pedestrianTwo;
    private FieldOfView fieldOfView;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        yield return null;
        Arrange();
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        DisableLoops();

        foreach (Pedestrian pedestrian in GameObject.FindObjectsOfType<Pedestrian>())
        {
            GameObject.Destroy(pedestrian);
        }

        Assert.Zero(GameObject.FindObjectsOfType<Pedestrian>().Length);
        pedestrianFactory = (PedestrianFactory)GameObject.FindObjectOfType(typeof(PedestrianFactory));
    }

    public override void Act()
    {
        pedestrianFactory.SpawnPedestrian();
        pedestrianTwo = GameObject.FindObjectOfType<Pedestrian>();
        fieldOfView = pedestrianTwo.GetComponentInChildren<FieldOfView>();
    }

    public override void Assertion()
    {
        Assert.IsNotNull(fieldOfView);
    }
}
