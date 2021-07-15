using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

public class WaitForFollowersBehaviour_IsLeaderAtDestination_ReturnsTrue_WhenLeaderIsWithinProximityOfDestination : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private WaitForFollowersBehaviour waitForFollowersBehaviour;
    private Vector3 evacuAgentPedestrianBaseLocation;
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
        waitForFollowersBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<WaitForFollowersBehaviour>();

        evacuAgentPedestrianBaseLocation = new Vector3(-5f, 0.5f, 20f);
        destination = new Vector3(-4.5f, 0.5f, 19.5f);

        SetPosition(evacuAgentPedestrianBase, evacuAgentPedestrianBaseLocation);
        evacuAgentPedestrianBase.GroupCollection.GroupDestination = destination;
    }

    public override void Act()
    {
        actualBool = waitForFollowersBehaviour.IsLeaderAtDestination();
    }

    public override void Assertion()
    {
        Assert.IsTrue(actualBool);
    }
}

public class WaitForFollowersBehaviour_IsLeaderAtDestination_ReturnsFalse_WhenLeaderIsNotWithinProximityOfDestination : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private WaitForFollowersBehaviour waitForFollowersBehaviour;
    private Vector3 evacuAgentPedestrianBaseLocation;
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
        waitForFollowersBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<WaitForFollowersBehaviour>();

        evacuAgentPedestrianBaseLocation = new Vector3(-5f, 0.5f, 20f);
        destination = new Vector3(20f, 0.5f, 5f);

        SetPosition(evacuAgentPedestrianBase, evacuAgentPedestrianBaseLocation);
        evacuAgentPedestrianBase.GroupCollection.GroupDestination = destination;
    }

    public override void Act()
    {
        actualBool = waitForFollowersBehaviour.IsLeaderAtDestination();
    }

    public override void Assertion()
    {
        Assert.IsFalse(actualBool);
    }
}

public class WaitForFollowersBehaviour_AreAllFolowersAtDestination_ReturnsFalse_WhenGroupMembersCountIsLessThanTotalGroupCount : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private GroupCollection groupCollection;
    private WaitForFollowersBehaviour waitForFollowersBehaviour;
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
        evacuAgentPedestrianBase = SpawnFriendGroupOfEvacuAgentPedestrians(2).First();
        waitForFollowersBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<WaitForFollowersBehaviour>();
        groupCollection = evacuAgentPedestrianBase.GroupCollection;

        groupCollection.TotalGroupCount = 3;
    }

    public override void Act()
    {
        actualBool = waitForFollowersBehaviour.AreAllFollowersAtDestination();
    }

    public override void Assertion()
    {
        Assert.IsFalse(actualBool);
    }
}

public class WaitForFollowersBehaviour_AreAllFolowersAtDestination_ReturnsFalse_WhenAGroupMemberIsOutsideOfTheAcceptableProximityToTheDestination : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private List<EvacuAgentPedestrianBase> groupMembers;
    private GroupCollection groupCollection;
    private WaitForFollowersBehaviour waitForFollowersBehaviour;
    private Vector3 destination;
    private List<Vector3> groupMemberLocations;
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
        groupMembers = SpawnFriendGroupOfEvacuAgentPedestrians(3);
        evacuAgentPedestrianBase = groupMembers.First();
        waitForFollowersBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<WaitForFollowersBehaviour>();
        groupCollection = evacuAgentPedestrianBase.GroupCollection;

        destination = new Vector3(15f, 0.5f, -3f);
        groupCollection.GroupDestination = destination;

        groupMemberLocations = new List<Vector3>
        {
            new Vector3(14.5f, 0.5f, -2.75f),
            new Vector3(15.5f, 0.5f, -3.25f),
            new Vector3(25f, 0.5f, -5f)
        };

        SetPositions(groupMembers, groupMemberLocations);
    }

    public override void Act()
    {
        actualBool = waitForFollowersBehaviour.AreAllFollowersAtDestination();
    }

    public override void Assertion()
    {
        Assert.IsFalse(actualBool);
    }
}

public class WaitForFollowersBehaviour_AreAllFolowersAtDestination_ReturnsTrue_WhenAllGroupMembersAreWithinAcceptableProximityToTheDestination : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private List<EvacuAgentPedestrianBase> groupMembers;
    private GroupCollection groupCollection;
    private WaitForFollowersBehaviour waitForFollowersBehaviour;
    private Vector3 destination;
    private List<Vector3> groupMemberLocations;
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
        groupMembers = SpawnFriendGroupOfEvacuAgentPedestrians(3);
        evacuAgentPedestrianBase = groupMembers.First();
        waitForFollowersBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<WaitForFollowersBehaviour>();
        groupCollection = evacuAgentPedestrianBase.GroupCollection;

        destination = new Vector3(15f, 0.5f, -3f);
        groupCollection.GroupDestination = destination;

        groupMemberLocations = new List<Vector3>
        {
            new Vector3(14.5f, 0.5f, -2.75f),
            new Vector3(15.5f, 0.5f, -3.25f),
            new Vector3(15.25f, 0.5f, -2.85f)
        };

        SetPositions(groupMembers, groupMemberLocations);
    }

    public override void Act()
    {
        actualBool = waitForFollowersBehaviour.AreAllFollowersAtDestination();
    }

    public override void Assertion()
    {
        Assert.IsTrue(actualBool);
    }
}

public class WaitForFollowersBehaviour_ShouldWaitForFollowersToSpawn_ReturnsTrue_WhenGroupCollectionCountIsZero : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private GroupCollection groupCollection;
    private WaitForFollowersBehaviour waitForFollowersBehaviour;
    private bool actualBool;
    private int expectedGroupMemberCount;

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
        waitForFollowersBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<WaitForFollowersBehaviour>();
        groupCollection = evacuAgentPedestrianBase.GroupCollection;

        groupCollection.GetGroupMembers().Clear();
        expectedGroupMemberCount = 0;
    }

    public override void Act()
    {
        actualBool = waitForFollowersBehaviour.ShouldWaitForFollowersToSpawn();
    }

    public override void Assertion()
    {
        Assert.AreEqual(expectedGroupMemberCount, groupCollection.GetGroupMembers().Count);
        Assert.IsTrue(actualBool);
    }
}

public class WaitForFollowersBehaviour_ShouldWaitForFollowersToSpawn_ReturnsFalse_WhenGroupCollectionCountIsGreaterThanZero : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private GroupCollection groupCollection;
    private WaitForFollowersBehaviour waitForFollowersBehaviour;
    private bool actualBool;
    private int expectedGroupMemberCount;

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
        evacuAgentPedestrianBase = SpawnFriendGroupOfEvacuAgentPedestrians(4).First();
        waitForFollowersBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<WaitForFollowersBehaviour>();
        groupCollection = evacuAgentPedestrianBase.GroupCollection;
        expectedGroupMemberCount = 4;
    }

    public override void Act()
    {
        actualBool = waitForFollowersBehaviour.ShouldWaitForFollowersToSpawn();
    }

    public override void Assertion()
    {
        Assert.AreEqual(expectedGroupMemberCount, groupCollection.GetGroupMembers().Count);
        Assert.IsFalse(actualBool);
    }
}

public class WaitForFollowersBehaviour_ShouldTriggerBehaviour_ReturnsTrue_WhenLeaderIsAtDestination_AndAllFollowersAreAtDestination : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase leader;
    private List<EvacuAgentPedestrianBase> groupMembers;
    private GroupCollection groupCollection;
    private WaitForFollowersBehaviour waitForFollowersBehaviour;
    private Vector3 destination;
    private List<Vector3> groupMemberLocations;
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
        groupMembers = SpawnFriendGroupOfEvacuAgentPedestrians(3);
        leader = groupMembers.First();
        waitForFollowersBehaviour = leader.GetComponentInChildren<WaitForFollowersBehaviour>();
        groupCollection = leader.GroupCollection;

        destination = new Vector3(15f, 0.5f, -3f);
        groupCollection.GroupDestination = destination;

        groupMemberLocations = new List<Vector3>
        {
            new Vector3(14.5f, 0.5f, -2.75f),
            new Vector3(15.5f, 0.5f, -3.25f),
            new Vector3(15.25f, 0.5f, -2.85f)
        };

        SetPositions(groupMembers, groupMemberLocations);
    }

    public override void Act()
    {
        actualBool = waitForFollowersBehaviour.ShouldTriggerBehaviour();
    }

    public override void Assertion()
    {
        Assert.IsTrue(actualBool);
    }
}

public class WaitForFollowersBehaviour_ShouldTriggerBehaviour_ReturnsFalse_WhenLeaderIsAtDestination_AndAllFollowersAreNotAtDestination : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase leader;
    private List<EvacuAgentPedestrianBase> groupMembers;
    private GroupCollection groupCollection;
    private WaitForFollowersBehaviour waitForFollowersBehaviour;
    private Vector3 destination;
    private List<Vector3> groupMemberLocations;
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
        groupMembers = SpawnFriendGroupOfEvacuAgentPedestrians(3);
        leader = groupMembers.First();
        waitForFollowersBehaviour = leader.GetComponentInChildren<WaitForFollowersBehaviour>();
        groupCollection = leader.GroupCollection;

        destination = new Vector3(15f, 0.5f, -3f);
        groupCollection.GroupDestination = destination;

        groupMemberLocations = new List<Vector3>
        {
            new Vector3(14.5f, 0.5f, -2.75f), // Leader at destination
            new Vector3(10.5f, 0.5f, -5.25f), // Follower not at destination
            new Vector3(15.25f, 0.5f, -2.85f) // Follower at destination
        };

        SetPositions(groupMembers, groupMemberLocations);
    }

    public override void Act()
    {
        actualBool = waitForFollowersBehaviour.ShouldTriggerBehaviour();
    }

    public override void Assertion()
    {
        Assert.IsFalse(actualBool);
    }
}

public class WaitForFollowersBehaviour_ShouldTriggerBehaviour_ReturnsFalse_WhenLeaderIsNotAtDestination_AndAllFollowersAreAtDestination : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase leader;
    private List<EvacuAgentPedestrianBase> groupMembers;
    private GroupCollection groupCollection;
    private WaitForFollowersBehaviour waitForFollowersBehaviour;
    private Vector3 destination;
    private List<Vector3> groupMemberLocations;
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
        groupMembers = SpawnFriendGroupOfEvacuAgentPedestrians(3);
        leader = groupMembers.First();
        waitForFollowersBehaviour = leader.GetComponentInChildren<WaitForFollowersBehaviour>();
        groupCollection = leader.GroupCollection;

        destination = new Vector3(15f, 0.5f, -3f);
        groupCollection.GroupDestination = destination;

        groupMemberLocations = new List<Vector3>
        {
            new Vector3(10.5f, 0.5f, -5.75f), // Leader not at destination
            new Vector3(15.5f, 0.5f, -3.25f), // Follower at at destination
            new Vector3(15.25f, 0.5f, -2.85f) // Follower at destination
        };

        SetPositions(groupMembers, groupMemberLocations);
    }

    public override void Act()
    {
        actualBool = waitForFollowersBehaviour.ShouldTriggerBehaviour();
    }

    public override void Assertion()
    {
        Assert.IsFalse(actualBool);
    }
}

public class WaitForFollowersBehaviour_PerformBehaviour_CorrectlyAdjustsFollowersSpeed_AndDestination : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase leader;
    private List<EvacuAgentPedestrianBase> groupMembers;
    private GroupCollection groupCollection;
    private WaitForFollowersBehaviour waitForFollowersBehaviour;
    private List<Vector3> groupPath;
    private List<Vector3> groupMemberInitalDestination;
    private List<NavMeshAgent> navMeshAgents;
    private float initialNavMeshSpeed;
    private float tolerance;

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
        // Set up group
        groupMembers = SpawnFriendGroupOfEvacuAgentPedestrians(3);
        leader = groupMembers.First();
        navMeshAgents = new List<NavMeshAgent>();
        tolerance = 0.06f;

        // Get behaviour and group collection
        waitForFollowersBehaviour = leader.GetComponentInChildren<WaitForFollowersBehaviour>();
        groupCollection = leader.GroupCollection;

        // Create a path to check that the method correctly sets follower destinations (compared to the inital destinations)
        groupPath = new List<Vector3>
        {
            new Vector3 (15f, 0f, -3f),
            new Vector3 (10f, 0f, -8f),
        };

        groupCollection.UpdatePath(groupPath);

        groupMemberInitalDestination = new List<Vector3>
        {
            new Vector3(15.5f, 0f, -3.25f),
            new Vector3(15.25f, 0f, -2.85f),
            new Vector3(-7.5f, 0f, 6f),
        };

        // Give navMeshAgents initial speed to check that the method is chaning the speed
        initialNavMeshSpeed = 15f;

        // Set Initial values for NavMeshAgent Speed, destination and isStopped (all should be changed in the method)
        for (int index = 0; index < groupMembers.Count; index++)
        {
            NavMeshAgent navMeshAgent = groupMembers[index].navMeshAgent;

            navMeshAgents.Add(navMeshAgent);
            navMeshAgent.speed = initialNavMeshSpeed;
            navMeshAgent.destination = groupMemberInitalDestination[index];
            navMeshAgent.isStopped = true;
        }
    }

    public override void Act()
    {
        waitForFollowersBehaviour.PerformBehaviour();
    }

    public override void Assertion()
    {
        foreach(NavMeshAgent navMeshAgent in navMeshAgents)
        {
            Assert.Less(navMeshAgent.speed, initialNavMeshSpeed);
            Assert.False(navMeshAgent.isStopped);
            AssertTwoVectorsAreEqualWithinTolerance(navMeshAgent.destination, groupPath[1], tolerance);
        }
    }
}
