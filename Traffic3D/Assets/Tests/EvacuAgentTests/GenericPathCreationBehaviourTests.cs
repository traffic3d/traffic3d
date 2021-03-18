using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;


public class GenericPathCreationBehaviour_PerformBehaviour_CorrectlyAddsElementsToPathOfPedestrianPoints : ArrangeActAssertStrategy
{
    private GameObject pedestrianGameObject;
    private Pedestrian pedestrian;
    private GenericPathCreationBehaviour genericPathCreationBehaviour;
    private List<PedestrianPoint> actualPathOfpedestrianPoints;
    private PedestrianType pedestrianType;
    private NavMeshAgent navMeshAgent;
    private int expectedNumberOfElements;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        Arrange();
        yield return null;
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        pedestrianGameObject = SpawnGameObjectWithInactivePedestrianScript();

        pedestrian = pedestrianGameObject.GetComponent<Pedestrian>();
        pedestrianType = PedestrianType.Worker;
        pedestrian.pedestrianType = pedestrianType;
        navMeshAgent = pedestrian.gameObject.AddComponent<NavMeshAgent>();

        genericPathCreationBehaviour = pedestrianGameObject.AddComponent<GenericPathCreationBehaviour>();
        expectedNumberOfElements = 1;
    }

    public override void Act()
    {
        genericPathCreationBehaviour.PerformBehaviour();
        actualPathOfpedestrianPoints = genericPathCreationBehaviour.PathOfPedestrianPoints;
    }

    public override void Assertion()
    {
        Assert.NotNull(actualPathOfpedestrianPoints);
        Assert.GreaterOrEqual(expectedNumberOfElements, actualPathOfpedestrianPoints.Count);
        Assert.AreEqual(navMeshAgent.destination, actualPathOfpedestrianPoints[0].GetPointLocation());
    }
}

public class GenericPathCreationBehaviour_ShouldTriggerbehaviour_ReturnsTrue_WhenPathOfPedestrianPointsIsNull : ArrangeActAssertStrategy
{
    private GameObject pedestrianGameObject;
    private Pedestrian pedestrian;
    private GenericPathCreationBehaviour genericPathCreationBehaviour;
    private bool actualBool;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        Arrange();
        yield return null;
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        pedestrianGameObject = SpawnGameObjectWithInactivePedestrianScript();
        pedestrian = pedestrianGameObject.GetComponent<Pedestrian>();
        pedestrian.gameObject.AddComponent<NavMeshAgent>();

        genericPathCreationBehaviour = pedestrianGameObject.AddComponent<GenericPathCreationBehaviour>();
        genericPathCreationBehaviour.PathOfPedestrianPoints = null;
    }

    public override void Act()
    {
        actualBool = genericPathCreationBehaviour.ShouldTriggerBehaviour();
    }

    public override void Assertion()
    {
        Assert.IsTrue(actualBool);
    }
}

public class GenericPathCreationBehaviour_ShouldTriggerbehaviour_ReturnsTrue_WhenPathOfPedestrianPointsHasZeroElements : ArrangeActAssertStrategy
{
    private GameObject pedestrianGameObject;
    private Pedestrian pedestrian;
    private GenericPathCreationBehaviour genericPathCreationBehaviour;
    private bool actualBool;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        Arrange();
        yield return null;
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        pedestrianGameObject = SpawnGameObjectWithInactivePedestrianScript();
        pedestrian = pedestrianGameObject.GetComponent<Pedestrian>();
        pedestrian.gameObject.AddComponent<NavMeshAgent>();

        genericPathCreationBehaviour = pedestrianGameObject.AddComponent<GenericPathCreationBehaviour>();
        genericPathCreationBehaviour.PathOfPedestrianPoints = new List<PedestrianPoint>();
    }

    public override void Act()
    {
        actualBool = genericPathCreationBehaviour.ShouldTriggerBehaviour();
    }

    public override void Assertion()
    {
        Assert.IsTrue(actualBool);
    }
}

public class GenericPathCreationBehaviour_ShouldTriggerbehaviour_ReturnsFalse_WhenPathOfPedestrianPoints_ISNotNull_AndContainsAtLeastOneElement : ArrangeActAssertStrategy
{
    private GameObject pedestrianGameObject;
    private Pedestrian pedestrian;
    private GenericPathCreationBehaviour genericPathCreationBehaviour;
    private bool actualBool;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        Arrange();
        yield return null;
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        pedestrianGameObject = SpawnGameObjectWithInactivePedestrianScript();
        pedestrian = pedestrianGameObject.GetComponent<Pedestrian>();
        pedestrian.gameObject.AddComponent<NavMeshAgent>();

        genericPathCreationBehaviour = pedestrianGameObject.AddComponent<GenericPathCreationBehaviour>();
        genericPathCreationBehaviour.PathOfPedestrianPoints = new List<PedestrianPoint>() { GameObject.FindObjectOfType<PedestrianPoint>() };
    }

    public override void Act()
    {
        actualBool = genericPathCreationBehaviour.ShouldTriggerBehaviour();
    }

    public override void Assertion()
    {
        Assert.IsFalse(actualBool);
    }
}
