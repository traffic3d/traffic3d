using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

public class BoidTargetSeekingComponet_ReturnsCorrectVelocity_BasedOnCurrentSteeringTarget : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private BoidTargetSeekingComponent boidTargetSeekingComponent;
    private FriendGroupBoidBehaviour friendGroupBoidBehaviour;
    private NavMeshAgent navMeshAgent;
    private Vector3 expectedVelocity;
    private Vector3 actualVelocity;
    private float originalWeight;
    private float testWeight;

    [UnityTest]
    public override IEnumerator PerformTest()
    {

        Arrange();
        yield return null;
        Act();
        Assertion();
        TearDown();
    }

    public override void Arrange()
    {
        originalWeight = EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_TARGET_SEEKING_WEIGHT;
        testWeight = 0.5f;
        EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_TARGET_SEEKING_WEIGHT = testWeight;

        evacuAgentPedestrianBase = SpawnFriendGroupOfEvacuAgentPedestrians(1).First();
        friendGroupBoidBehaviour = (FriendGroupBoidBehaviour)BoidTestsSetupHelper.GetBoidBehaviourStrategyBasesFromEvacuAgentPedestrianBases(new List<EvacuAgentPedestrianBase> { evacuAgentPedestrianBase }).First();
        boidTargetSeekingComponent = evacuAgentPedestrianBase.GetComponentInChildren<BoidTargetSeekingComponent>();
        navMeshAgent = evacuAgentPedestrianBase.navMeshAgent;

        SetPosition(friendGroupBoidBehaviour, new Vector3(2f, 10f, -3.25f));

        // Add steering target to NavMeshAgent
        navMeshAgent.SetDestination(new Vector3(20f, 2f, 5f));

        expectedVelocity = new Vector3(0.4061f, -0.2244f, 0.1861f);
    }

    public override void Act()
    {
        actualVelocity = boidTargetSeekingComponent.CalculateComponentVelocity(friendGroupBoidBehaviour);
    }

    public override void Assertion()
    {
        AssertTwoVectorsAreEqualWithinTolerance(actualVelocity, expectedVelocity, floatingPointTolerance);
    }

    public void TearDown()
    {
        EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_TARGET_SEEKING_WEIGHT = originalWeight;
    }
}
