using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

public class FollowerDestinationUpdateBehaviour_ShouldTriggerBehaviour_ReturnsTrue_WhenGroupDestinationDoesNotEqualCurrentDestination : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private FollowerDestinationUpdateBehaviour followerDestinationUpdateBehaviour;
    private bool actualBool;
    private Vector3 locationOne;
    private Vector3 locationTwo;

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
        // Spawn 2 pedestrians and take the second as that is a follower type (leaders do not have this behaviour)
        evacuAgentPedestrianBase = SpawnFriendGroupOfEvacuAgentPedestrians(2)[1];
        followerDestinationUpdateBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<FollowerDestinationUpdateBehaviour>();

        locationOne = new Vector3(1f, 0f, 20f);
        locationTwo = new Vector3(3f, 0f, -17f);

        // Set locationOne as the GroupDestination
        evacuAgentPedestrianBase.GroupCollection.GroupDestination = locationOne;
    }

    public override void Act()
    {
        // Set locationTwo as the new GroupDestination so that is is different fromt he currentDestination in FollowerDestinationUpdateBehaviour
        evacuAgentPedestrianBase.GroupCollection.GroupDestination = locationTwo;
        actualBool = followerDestinationUpdateBehaviour.ShouldTriggerBehaviour();
    }

    public override void Assertion()
    {
        Assert.True(actualBool);
    }
}

public class FollowerDestinationUpdateBehaviour_ShouldTriggerBehaviour_ReturnsFalse_WhenGroupDestinationEqualsCurrentDestination : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private FollowerDestinationUpdateBehaviour followerDestinationUpdateBehaviour;
    private bool actualBool;
    private Vector3 locationOne;

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
        // Spawn 2 pedestrians and take the second as that is a follower type (leaders do not have this behaviour)
        evacuAgentPedestrianBase = SpawnFriendGroupOfEvacuAgentPedestrians(2)[1];
        followerDestinationUpdateBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<FollowerDestinationUpdateBehaviour>();

        locationOne = new Vector3(1f, 0f, 20f);

        // Set locationOne as the GroupDestination
        evacuAgentPedestrianBase.GroupCollection.GroupDestination = locationOne;
    }

    public override void Act()
    {
        actualBool = followerDestinationUpdateBehaviour.ShouldTriggerBehaviour();
    }

    public override void Assertion()
    {
        Assert.False(actualBool);
    }
}

public class FollowerDestinationUpdateBehaviour_PerformBehaviour_CorrectlySetsNewDestination : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private FollowerDestinationUpdateBehaviour followerDestinationUpdateBehaviour;
    private NavMeshAgent navMeshAgent;
    private Vector3 locationOne;
    private Vector3 locationTwo;

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
        // Spawn 2 pedestrians and take the second as that is a follower type (leaders do not have this behaviour)
        evacuAgentPedestrianBase = SpawnFriendGroupOfEvacuAgentPedestrians(2)[1];
        followerDestinationUpdateBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<FollowerDestinationUpdateBehaviour>();

        locationOne = new Vector3(1f, 0.005f, 20f);
        locationTwo = new Vector3(3f, 0.005f, -17f);

        navMeshAgent = evacuAgentPedestrianBase.navMeshAgent;

        // Set locationOne as the GroupDestination
        evacuAgentPedestrianBase.GroupCollection.GroupDestination = locationOne;
    }

    public override void Act()
    {
        AssertTwoVectorsAreEqualWithinTolerance(navMeshAgent.destination, locationOne, floatingPointTolerance);

        // Set locationTwo as the new GroupDestination so that is is different fromt he currentDestination in FollowerDestinationUpdateBehaviour
        evacuAgentPedestrianBase.GroupCollection.GroupDestination = locationTwo;
        followerDestinationUpdateBehaviour.PerformBehaviour();
    }

    public override void Assertion()
    {
        AssertTwoVectorsAreEqualWithinTolerance(navMeshAgent.destination, locationTwo, floatingPointTolerance);
    }
}
