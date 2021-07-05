using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BoidSeparationComponent_ReturnsVelocityOfZero_WhenNeighboursCountIsZero : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private BoidSeparationComponent boidSeparationComponent;
    private FriendGroupBoidBehaviour friendGroupBoidBehaviour;
    private Vector3 actualBoidSeparationComponentResult;
    private Vector3 expectedBoidSeparationComponentResult;

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
        friendGroupBoidBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<FriendGroupBoidBehaviour>();
        boidSeparationComponent = evacuAgentPedestrianBase.GetComponentInChildren<BoidSeparationComponent>();
        expectedBoidSeparationComponentResult = Vector3.zero;
    }

    public override void Act()
    {
        actualBoidSeparationComponentResult = boidSeparationComponent.CalculateComponentVelocity(friendGroupBoidBehaviour);
    }

    public override void Assertion()
    {
        Assert.Zero(friendGroupBoidBehaviour.Neighbours.Count);
        Assert.AreEqual(expectedBoidSeparationComponentResult, actualBoidSeparationComponentResult);
    }
}

public class BoidSeparationComponent_ReturnsCorrectVelocity_WhenNeighboursCountIsTwo : ArrangeActAssertStrategy
{
    private BoidSeparationComponent boidSeparationComponent;
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private FriendGroupBoidBehaviour friendGroupBoidBehaviour;
    private List<EvacuAgentPedestrianBase> groupMembers;
    private List<BoidBehaviourStrategyBase> neighbours;
    private KeyValuePair<BoidBehaviourStrategyBase, List<BoidBehaviourStrategyBase>> leaderAndNeighbours;
    private Vector3 actualBoidSeparationComponentResult;
    private Vector3 expectedBoidSeparationComponentResult;
    private List<Vector3> neighbourLocations;
    private float originalBoidSeparationComponentWeight;
    private float testBoidSeparationComponentWeight;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        yield return null;
        Arrange();
        Act();
        Assertion();
        TearDown();
    }

    public override void Arrange()
    {
        // Alter weight so that the test does not need to  be canged if weights change
        originalBoidSeparationComponentWeight = EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_SEPARATION_WEIGHT;
        testBoidSeparationComponentWeight = 0.5f;
        EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_SEPARATION_WEIGHT = testBoidSeparationComponentWeight;

        // Set up main pedestrian and group
        groupMembers = SpawnFriendGroupOfEvacuAgentPedestrians(3);
        evacuAgentPedestrianBase = groupMembers.First();
        neighbours = BoidTestsSetupHelper.GetBoidBehaviourStrategyBasesFromEvacuAgentPedestrianBases(groupMembers);
        leaderAndNeighbours = BoidTestsSetupHelper.SeparateLeaderAndGroupMembers(neighbours, neighbours.First());
        friendGroupBoidBehaviour = (FriendGroupBoidBehaviour)leaderAndNeighbours.Key;
        friendGroupBoidBehaviour.Neighbours = leaderAndNeighbours.Value;

        // Set up neighbour locations used for separation velocity calculations
        neighbourLocations = new List<Vector3>
        {
            new Vector3(10f, 0f, 12f),
            new Vector3(-4f, 0f, 2f),
            new Vector3(-6f, 0f, 5f),
        };

        SetPositions(neighbours, neighbourLocations);

        // Get the boid component being tested
        boidSeparationComponent = evacuAgentPedestrianBase.GetComponentInChildren<BoidSeparationComponent>();

        expectedBoidSeparationComponentResult = new Vector3(0.0014f, 0f, 0.0008f);
    }

    public override void Act()
    {
        actualBoidSeparationComponentResult = boidSeparationComponent.CalculateComponentVelocity(friendGroupBoidBehaviour);
    }

    public override void Assertion()
    {
        AssertTwoVectorsAreEqualWithinTolerance(actualBoidSeparationComponentResult, expectedBoidSeparationComponentResult, floatingPointTolerance);
    }

    public void TearDown()
    {
        EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_SEPARATION_WEIGHT = originalBoidSeparationComponentWeight;
    }
}
