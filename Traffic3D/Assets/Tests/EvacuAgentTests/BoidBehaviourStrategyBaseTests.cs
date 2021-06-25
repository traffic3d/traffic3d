using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

public class BoidBehaviourStrategyBase_ShouldBoidLogicBeActive_SetsVelocityToZero_WhenShouldBoidLogicBeOnIsSetToFalse : ArrangeActAssertStrategy
{
    private Vector3 velocityBeforeTest;
    private Vector3 expectedVelocity;
    private Vector3 actualVelocity;
    private FriendGroupBoidBehaviour friendGroupBoidBehaviour;
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;

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
        expectedVelocity = Vector3.zero;
        velocityBeforeTest = new Vector3(1f, 0, 1f); // This is arbitrary, we just want to test that velocity is set to Zero here
        evacuAgentPedestrianBase = SpawnFriendGroupOfEvacuAgentPedestrians(1).First();
        friendGroupBoidBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<FriendGroupBoidBehaviour>();
        friendGroupBoidBehaviour.Velocity = velocityBeforeTest;
    }

    public override void Act()
    {
        friendGroupBoidBehaviour.ShouldBoidLogicBeActive(false);
        actualVelocity = friendGroupBoidBehaviour.Velocity;
    }

    public override void Assertion()
    {
        Assert.AreEqual(expectedVelocity, actualVelocity);
    }
}

public class BoidBehaviourStrategyBase_UpdateNeighbours_CorrectlyAddsVisiblePedestriansToNeighboursAndNonGroupNeighbours: ArrangeActAssertStrategy
{
    private FriendGroupBoidBehaviour observerFriendGroupBoidBehaviour;
    private EvacuAgentPedestrianBase observerEvacuAgentPedestrianBase;
    private List<Pedestrian> allVisiblePedestrians;
    private List<EvacuAgentPedestrianBase> observerGroup;
    private List<EvacuAgentPedestrianBase> nonObserverGroup;
    private List<BoidBehaviourStrategyBase> expectedNeighbours;
    private List<BoidBehaviourStrategyBase> expectedNonGroupNeighbours;
    private List<BoidBehaviourStrategyBase> actualNeighbours;
    private List<BoidBehaviourStrategyBase> actualNonGroupNeighbours;

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
        // Create groups of EvacuAgent Pedestrians
        observerGroup = SpawnFriendGroupOfEvacuAgentPedestrians(3);
        nonObserverGroup = SpawnFriendGroupOfEvacuAgentPedestrians(3);

        // Get an observer, so that the friendGroupBoidBehaviour of this observer can be used. The obserer is removed to not observe themselves
        observerEvacuAgentPedestrianBase = observerGroup.First();
        observerFriendGroupBoidBehaviour = observerEvacuAgentPedestrianBase.GetComponentInChildren<FriendGroupBoidBehaviour>();
        observerGroup.Remove(observerEvacuAgentPedestrianBase);

        // Set all visible pedestrians in the observers field of view
        allVisiblePedestrians = BoidTestsSetupHelper.CombineEvacuAgentCollectionsIntoPedestrianList(new List<List<EvacuAgentPedestrianBase>>() { observerGroup, nonObserverGroup });
        observerEvacuAgentPedestrianBase.fieldOfView.allVisiblePedestrians = allVisiblePedestrians;

        // Set the expected visible group and non group member lists
        expectedNeighbours = new List<BoidBehaviourStrategyBase>();
        expectedNonGroupNeighbours = new List<BoidBehaviourStrategyBase>();
        BoidTestsSetupHelper.PopulateBoidBehaviourListFromEvacuAgentPedestrianList(observerGroup, expectedNeighbours);
        BoidTestsSetupHelper.PopulateBoidBehaviourListFromEvacuAgentPedestrianList(nonObserverGroup, expectedNonGroupNeighbours);
    }

    public override void Act()
    {
        observerFriendGroupBoidBehaviour.UpdateNeighbours();
        actualNeighbours = observerFriendGroupBoidBehaviour.Neighbours;
        actualNonGroupNeighbours = observerFriendGroupBoidBehaviour.NonGroupNeighbours;
    }

    public override void Assertion()
    {
        Assert.AreEqual(expectedNeighbours, actualNeighbours);
        Assert.AreEqual(expectedNonGroupNeighbours, actualNonGroupNeighbours);
    }
}

public class BoidBehaviourStrategyBase_LimitVelocity_CorrectlyLimitsTheMagnitudeOfTheVelocity_WhenVelocityExceedsMaxSpeed : ArrangeActAssertStrategy
{
    private Vector3 velocityBeforeLimit;
    private Vector3 expectedVelocity;
    private Vector3 actualVelocity;
    private FriendGroupBoidBehaviour friendGroupBoidBehaviour;
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;

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
        velocityBeforeLimit = new Vector3(25f, 0f, 25f); // This is arbitrary, we use a velocity that is guarenteed to be over our velocity limit
        evacuAgentPedestrianBase = SpawnFriendGroupOfEvacuAgentPedestrians(1).First();
        friendGroupBoidBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<FriendGroupBoidBehaviour>();

        expectedVelocity = new Vector3(1.697f, 0f, 1.697f);
    }

    public override void Act()
    {
        actualVelocity = friendGroupBoidBehaviour.LimitVelocity(velocityBeforeLimit);
    }

    public override void Assertion()
    {
        AssertTwoVectorsAreEqualWithinTolerance(actualVelocity, expectedVelocity, floatingPointTolerance);
    }
}

public class BoidBehaviourStrategyBase_LimitVelocity_DoesNotLimitTheMagnitudeOfTheVelocity_WhenVelocityIsBelowMaxSpeed: ArrangeActAssertStrategy
{
    private Vector3 velocityBeforeLimit;
    private Vector3 expectedVelocity;
    private Vector3 actualVelocity;
    private FriendGroupBoidBehaviour friendGroupBoidBehaviour;
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;

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
        velocityBeforeLimit = new Vector3(1f, 0f, 1f); // This is arbitrary, we use a velocity that is guarenteed to be over our velocity limit
        evacuAgentPedestrianBase = SpawnFriendGroupOfEvacuAgentPedestrians(1).First();
        friendGroupBoidBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<FriendGroupBoidBehaviour>();

        expectedVelocity = velocityBeforeLimit;
    }

    public override void Act()
    {
        actualVelocity = friendGroupBoidBehaviour.LimitVelocity(velocityBeforeLimit);
    }

    public override void Assertion()
    {
        Assert.Less(velocityBeforeLimit.magnitude, friendGroupBoidBehaviour.maxSpeedMetresSecond);
        AssertTwoVectorsAreEqualWithinTolerance(actualVelocity, expectedVelocity, floatingPointTolerance);
    }
}

public class BoidBehaviourStrategyBase_CalculateNeighbourPoint_CalculatesTheCorrectNeighbourCentrePoint : ArrangeActAssertStrategy
{
    private Vector3 expectedNeighbourCentre;
    private Vector3 actualNeighbourCentre;
    private List<Vector3> locations;
    private FriendGroupBoidBehaviour friendGroupBoidBehaviour;
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private List<EvacuAgentPedestrianBase> groupMembers;
    private List<BoidBehaviourStrategyBase> neighbours;

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
        locations = new List<Vector3>
        {
            new Vector3(1f, 0f, 1f),
            new Vector3(2f, 0f, 1f),
            new Vector3(1f, 0f, 3f),
            new Vector3(4f, 0f, 4f)
        };

        expectedNeighbourCentre = new Vector3(2f, 0f, 2.25f);

        groupMembers = SpawnFriendGroupOfEvacuAgentPedestrians(4);
        neighbours = BoidTestsSetupHelper.GetBoidBehaviourStrategyBasesFromEvacuAgentPedestrianBases(groupMembers);

        evacuAgentPedestrianBase = groupMembers.First();
        friendGroupBoidBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<FriendGroupBoidBehaviour>();

        // Place all group members in given positions
        for(int index = 0; index < locations.Count; index++)
        {
            neighbours[index].transform.position = locations[index];
        }

        // Remove self from neighbours list as FriendGroupBoidBehaviour will consider itself in centre calculation in CalculateNeighbourPoint()
        neighbours.Remove(friendGroupBoidBehaviour);

        friendGroupBoidBehaviour.Neighbours = neighbours;
    }

    public override void Act()
    {
        friendGroupBoidBehaviour.CalculateNeighbourPoint();
        actualNeighbourCentre = friendGroupBoidBehaviour.NeighbourCenter;
    }

    public override void Assertion()
    {
        AssertTwoVectorsAreEqualWithinTolerance(actualNeighbourCentre, expectedNeighbourCentre, floatingPointTolerance);
    }
}
