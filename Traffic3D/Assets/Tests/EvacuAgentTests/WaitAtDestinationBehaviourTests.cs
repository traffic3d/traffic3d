using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class WaitAtDestinationBehaviourTests_ShouldTriggerBehaviour_ReturnsTrue_WhenInProximityOfDestination : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private GroupCollection groupCollection;
    private WaitAtDestinationBehaviour waitAtDestinationBehaviour;
    private Vector3 currentLocation;
    private Vector3 destination;
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
        waitAtDestinationBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<WaitAtDestinationBehaviour>();
        groupCollection = evacuAgentPedestrianBase.GroupCollection;

        currentLocation = new Vector3(-15f, 0f, 3.75f);
        destination = new Vector3(-13.75f, 0f, 5f);

        groupCollection.GroupDestination = destination;
        SetPosition(evacuAgentPedestrianBase, currentLocation);
    }

    public override void Act()
    {
        waitAtDestinationBehaviour.RadiusToDestination = 3f;
        actualBool = waitAtDestinationBehaviour.ShouldTriggerBehaviour();
    }

    public override void Assertion()
    {
        Assert.IsTrue(actualBool);
    }
}

public class WaitAtDestinationBehaviourTests_ShouldTriggerBehaviour_ReturnsFalse_WhenNotInProximityOfDestination : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private GroupCollection groupCollection;
    private WaitAtDestinationBehaviour waitAtDestinationBehaviour;
    private Vector3 currentLocation;
    private Vector3 destination;
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
        waitAtDestinationBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<WaitAtDestinationBehaviour>();
        groupCollection = evacuAgentPedestrianBase.GroupCollection;

        currentLocation = new Vector3(-15f, 0f, 3.75f);
        destination = new Vector3(13.75f, 0f, 8f);

        groupCollection.GroupDestination = destination;
        SetPosition(evacuAgentPedestrianBase, currentLocation);
    }

    public override void Act()
    {
        actualBool = waitAtDestinationBehaviour.ShouldTriggerBehaviour();
    }

    public override void Assertion()
    {
        Assert.IsFalse(actualBool);
    }
}

public class WaitAtDestinationBehaviourTests_PerformBehaviour_CorrectlyChangesSpeedToMatchLeader_AndSetsIsMovementStoppedToTrue : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase followerEvacuAgentPedestrianBase;
    private WaitAtDestinationBehaviour waitAtDestinationBehaviour;
    private BoidManager followerBoidManager;
    private int expectedSpeed;

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
        followerEvacuAgentPedestrianBase = SpawnFriendGroupOfEvacuAgentPedestrians(2)[1];
        waitAtDestinationBehaviour = followerEvacuAgentPedestrianBase.GetComponentInChildren<WaitAtDestinationBehaviour>();
        followerBoidManager = followerEvacuAgentPedestrianBase.GetComponentInChildren<BoidManager>();
        expectedSpeed = 2;
    }

    public override void Act()
    {
        waitAtDestinationBehaviour.PerformBehaviour();
    }

    public override void Assertion()
    {
        Assert.AreEqual(expectedSpeed, followerEvacuAgentPedestrianBase.navMeshAgent.speed);
        Assert.IsTrue(followerEvacuAgentPedestrianBase.navMeshAgent.isStopped);
        Assert.IsTrue(followerBoidManager.isBoidMovementStopped);
    }
}
