using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;


public class GenericPathCreationBehaviour_PerformBehaviour_CorrectlyAddsElementsToPathOfPedestrianPoints : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private GenericPathCreationBehaviour genericPathCreationBehaviour;
    private List<Vector3> actualPathOfpedestrianPoints;
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
        evacuAgentPedestrianBase = SpawnFriendGroupOfEvacuAgentPedestrians(1).First();
        genericPathCreationBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<GenericPathCreationBehaviour>();
        navMeshAgent = evacuAgentPedestrianBase.navMeshAgent;
        expectedNumberOfElements = 5;
    }

    public override void Act()
    {
        genericPathCreationBehaviour.PerformBehaviour();
        actualPathOfpedestrianPoints = genericPathCreationBehaviour.Path;
    }

    public override void Assertion()
    {
        Assert.NotNull(actualPathOfpedestrianPoints);
        Assert.GreaterOrEqual(expectedNumberOfElements, actualPathOfpedestrianPoints.Count);
        Assert.AreEqual(navMeshAgent.destination, actualPathOfpedestrianPoints[0]);
    }
}

public class GenericPathCreationBehaviour_ShouldTriggerbehaviour_ReturnsTrue_WhenPathOfPedestrianPointsIsNull : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
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
        evacuAgentPedestrianBase = SpawnFriendGroupOfEvacuAgentPedestrians(1).First();
        genericPathCreationBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<GenericPathCreationBehaviour>();
        genericPathCreationBehaviour.Path = null;
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
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
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
        evacuAgentPedestrianBase = SpawnFriendGroupOfEvacuAgentPedestrians(1).First();
        genericPathCreationBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<GenericPathCreationBehaviour>();
        genericPathCreationBehaviour.Path = new List<Vector3>();
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
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private GenericPathCreationBehaviour genericPathCreationBehaviour;
    private bool actualBool;
    private PedestrianPoint pedestrianPoint;
    private PedestrianPoint pedestrianPointTwo;

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
        evacuAgentPedestrianBase = SpawnFriendGroupOfEvacuAgentPedestrians(1).First();
        genericPathCreationBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<GenericPathCreationBehaviour>();
        pedestrianPoint = GameObject.FindObjectOfType<PedestrianPoint>();
        pedestrianPointTwo = GameObject.FindObjectOfType<PedestrianPoint>();
        evacuAgentPedestrianBase.GroupCollection.UpdatePath(new List<Vector3>() { pedestrianPoint.GetPointLocation(), pedestrianPointTwo.GetPointLocation() });
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
