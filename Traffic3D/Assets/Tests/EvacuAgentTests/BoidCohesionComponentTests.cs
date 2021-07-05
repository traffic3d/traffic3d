using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BoidCohesionComponent_ReturnsVelocityOfZero_WhenNeighboursCountIsZero : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private BoidCohesionComponent boidCohesionComponent;
    private FriendGroupBoidBehaviour friendGroupBoidBehaviour;
    private Vector3 actualBoidCohesionComponentResult;
    private Vector3 expectedBoidCohestionComponentResult;

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
        boidCohesionComponent = evacuAgentPedestrianBase.GetComponentInChildren<BoidCohesionComponent>();
        expectedBoidCohestionComponentResult = Vector3.zero;
    }

    public override void Act()
    {
        actualBoidCohesionComponentResult = boidCohesionComponent.CalculateComponentVelocity(friendGroupBoidBehaviour);
    }

    public override void Assertion()
    {
        Assert.Zero(friendGroupBoidBehaviour.Neighbours.Count);
        Assert.AreEqual(expectedBoidCohestionComponentResult, actualBoidCohesionComponentResult);
    }
}

public class BoidCohesionComponent_ReturnsCorrectVelocity_WhenNeighboursCountIsTwo : ArrangeActAssertStrategy
{
    private BoidCohesionComponent boidCohesionComponent;
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private FriendGroupBoidBehaviour friendGroupBoidBehaviour;
    private List<EvacuAgentPedestrianBase> groupMembers;
    private List<BoidBehaviourStrategyBase> neighbours;
    private KeyValuePair<BoidBehaviourStrategyBase, List<BoidBehaviourStrategyBase>> leaderAndNeighbours;
    private Vector3 actualBoidCohesionComponentResult;
    private Vector3 expectedBoidCohestionComponentResult;
    private float originalBoidCohesionComponentWeight;
    private float testBoidCohesionComponentWeight;
    private int expectedNeighbourCount;

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
        originalBoidCohesionComponentWeight = EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_COHESION_WEIGHT;
        testBoidCohesionComponentWeight = 0.5f;
        EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_COHESION_WEIGHT = testBoidCohesionComponentWeight;

        // Set up main pedestrian and group
        groupMembers = SpawnFriendGroupOfEvacuAgentPedestrians(3);
        evacuAgentPedestrianBase = groupMembers.First();
        neighbours = BoidTestsSetupHelper.GetBoidBehaviourStrategyBasesFromEvacuAgentPedestrianBases(groupMembers);
        leaderAndNeighbours = BoidTestsSetupHelper.SeparateLeaderAndGroupMembers(neighbours, neighbours.First());
        friendGroupBoidBehaviour = (FriendGroupBoidBehaviour)leaderAndNeighbours.Key;
        friendGroupBoidBehaviour.Neighbours = leaderAndNeighbours.Value;

        // Set up neighbour centre used for velocity calculation
        BoidTestsSetupHelper.SetNeighbourCentre(friendGroupBoidBehaviour, new Vector3(5f, 0f, 2f));
        SetPosition(friendGroupBoidBehaviour, new Vector3(10f, 0f, 12f));

        // Get the boid component being tested
        boidCohesionComponent = evacuAgentPedestrianBase.GetComponentInChildren<BoidCohesionComponent>();

        expectedNeighbourCount = 2;
        expectedBoidCohestionComponentResult = new Vector3(-2.5f, 0f, -5f);
    }

    public override void Act()
    {
        actualBoidCohesionComponentResult = boidCohesionComponent.CalculateComponentVelocity(friendGroupBoidBehaviour);
    }

    public override void Assertion()
    {
        Assert.True(expectedNeighbourCount == friendGroupBoidBehaviour.Neighbours.Count);
        AssertTwoVectorsAreEqualWithinTolerance(actualBoidCohesionComponentResult, expectedBoidCohestionComponentResult, floatingPointTolerance);
    }

    public void TearDown()
    {
        EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_COHESION_WEIGHT = originalBoidCohesionComponentWeight;
    }
}
