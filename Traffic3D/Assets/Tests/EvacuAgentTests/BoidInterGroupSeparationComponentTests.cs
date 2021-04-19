using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BoidInterGroupSeparationComponent_GetPositionsOfNonGroupPedestriansInAssociatedSubGroups_PopulatesPositionOfVisibleMembersInEachSubGroupCorrectly : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private BoidInterGroupSeparationComponent boidInterGroupSeparationComponent;
    private Dictionary<GroupCollection, List<Vector3>> expectedDictionary;
    private Dictionary<GroupCollection, List<Vector3>> actualDictionary;
    private List<List<EvacuAgentPedestrianBase>> visibleEvacuAgentBaseGroups;
    private List<BoidBehaviourStrategyBase> visibleNonGroupPedestrians;
    private List<List<Vector3>> groupPositions;
    private List<Vector3> groupOnePositions;
    private List<Vector3> groupTwoPositions;
    private List<Vector3> groupThreePositions;

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
        boidInterGroupSeparationComponent = evacuAgentPedestrianBase.GetComponentInChildren<BoidInterGroupSeparationComponent>();


        // Spawn the groups
        visibleEvacuAgentBaseGroups = new List<List<EvacuAgentPedestrianBase>>
        {
            SpawnFriendGroupOfEvacuAgentPedestrians(3),
            SpawnFriendGroupOfEvacuAgentPedestrians(2),
            SpawnFriendGroupOfEvacuAgentPedestrians(4),
        };

        // Add all members of all groups as a list of visible pedestrians
        visibleNonGroupPedestrians = new List<BoidBehaviourStrategyBase>();

        foreach (List<EvacuAgentPedestrianBase> evacuAgentPedestrianBases in visibleEvacuAgentBaseGroups)
        {
            visibleNonGroupPedestrians.AddRange(BoidTestsSetupHelper.GetBoidBehaviourStrategyBasesFromEvacuAgentPedestrianBases(evacuAgentPedestrianBases));
        }

        // Set positions of all members of each group
        groupPositions = BoidInterGroupSeparationComponentTestsHelper.GetPositionsForGroups();

        for (int index = 0; index < visibleEvacuAgentBaseGroups.Count; index++)
        {
            SetPositions(visibleEvacuAgentBaseGroups[index], groupPositions[index]);
        }


        // Created expected dicrionary of group collection and the associated members positions
        expectedDictionary = new Dictionary<GroupCollection, List<Vector3>>
        {
            { visibleEvacuAgentBaseGroups[0][0].GroupCollection, groupPositions[0] },
            { visibleEvacuAgentBaseGroups[1][0].GroupCollection, groupPositions[1] },
            { visibleEvacuAgentBaseGroups[2][0].GroupCollection, groupPositions[2] }
        };
    }

    public override void Act()
    {
        boidInterGroupSeparationComponent.GetPositionsOfNonGroupPedestriansInAssociatedSubGroups(visibleNonGroupPedestrians);
        actualDictionary = boidInterGroupSeparationComponent.positionOfVisibleMembersInEachSubGroup;
    }

    public override void Assertion()
    {
        Assert.That(actualDictionary, Is.EqualTo(expectedDictionary));
    }
}

public class BoidInterGroupSeparationComponent_FindCentresOfAllVisbleNonGroupPedestrianSubGroups_FindsCentresCorrectly : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private BoidInterGroupSeparationComponent boidInterGroupSeparationComponent;
    private List<List<EvacuAgentPedestrianBase>> visibleEvacuAgentBaseGroups;
    private List<BoidBehaviourStrategyBase> visibleNonGroupPedestrians;
    private List<Vector3> expectedCentres;
    private List<Vector3> actualCentres;
    private List<List<Vector3>> groupPositions;

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
        boidInterGroupSeparationComponent = evacuAgentPedestrianBase.GetComponentInChildren<BoidInterGroupSeparationComponent>();

        // Spawn the groups
        visibleEvacuAgentBaseGroups = new List<List<EvacuAgentPedestrianBase>>
        {
            SpawnFriendGroupOfEvacuAgentPedestrians(3),
            SpawnFriendGroupOfEvacuAgentPedestrians(2),
            SpawnFriendGroupOfEvacuAgentPedestrians(4),
        };

        // Add all members of all groups as a list of visible pedestrians
        visibleNonGroupPedestrians = new List<BoidBehaviourStrategyBase>();

        foreach (List<EvacuAgentPedestrianBase> evacuAgentPedestrianBases in visibleEvacuAgentBaseGroups)
        {
            visibleNonGroupPedestrians.AddRange(BoidTestsSetupHelper.GetBoidBehaviourStrategyBasesFromEvacuAgentPedestrianBases(evacuAgentPedestrianBases));
        }

        // Set positions of all members of each group
        groupPositions = BoidInterGroupSeparationComponentTestsHelper.GetPositionsForGroups();

        for (int index = 0; index < visibleEvacuAgentBaseGroups.Count; index++)
        {
            SetPositions(visibleEvacuAgentBaseGroups[index], groupPositions[index]);
        }

        expectedCentres = new List<Vector3>
        {
            new Vector3(2f, 0f, 2f),
            new Vector3(0f, 0f, 0.5f),
            new Vector3(2.25f, 0f, -0.25f),
        };
    }

    public override void Act()
    {
        // This method call is necessary to populate positionOfVisibleMembersInEachSubGroup
        boidInterGroupSeparationComponent.GetPositionsOfNonGroupPedestriansInAssociatedSubGroups(visibleNonGroupPedestrians);
        actualCentres = boidInterGroupSeparationComponent.FindCentresOfAllVisbleNonGroupPedestrianSubGroups();
    }

    public override void Assertion()
    {
        Assert.That(actualCentres, Is.EqualTo(expectedCentres));
    }
}

public class BoidInterGroupSeparationComponent_FindCentreOfSingleSubGroup_FindsCentreCorrectly : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private BoidInterGroupSeparationComponent boidInterGroupSeparationComponent;
    private Vector3 expectedCentre;
    private Vector3 actualCentre;
    private List<Vector3> subGroup;

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
        boidInterGroupSeparationComponent = evacuAgentPedestrianBase.GetComponentInChildren<BoidInterGroupSeparationComponent>();

        subGroup = new List<Vector3>
        {
            new Vector3(-1f, 0f, 7f),
            new Vector3(5f, 0f, -8.5f),
            new Vector3(2f, 0f, 4f),
            new Vector3(3.75f, 0f, 0.25f),
        };

        expectedCentre = new Vector3(2.4375f, 0f, 0.6875f);
    }

    public override void Act()
    {
        actualCentre = boidInterGroupSeparationComponent.FindCentreOfSingleSubGroup(subGroup);
    }

    public override void Assertion()
    {
        AssertTwoVectorsAreEqualWithinTolerance(actualCentre, expectedCentre, floatingPointTolerance);
    }
}

public class BoidInterGroupSeparationComponent_CalculateComponentVelocity_CalculatesCorrectVelocity : ArrangeActAssertStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private FriendGroupBoidBehaviour friendGroupBoidBehaviour;
    private BoidInterGroupSeparationComponent boidInterGroupSeparationComponent;
    private List<List<EvacuAgentPedestrianBase>> visibleEvacuAgentBaseGroups;
    private List<BoidBehaviourStrategyBase> visibleNonGroupPedestrians;
    private Vector3 expectedVelocity;
    private Vector3 actualVelocity;
    private List<List<Vector3>> groupPositions;
    private float originalWeight;
    private float testWeight;

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
        originalWeight = EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_INTER_GROUP_SEPARATION_WEIGHT;
        testWeight = 0.5f;
        EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_INTER_GROUP_SEPARATION_WEIGHT = testWeight;

        evacuAgentPedestrianBase = SpawnFriendGroupOfEvacuAgentPedestrians(1).First();
        boidInterGroupSeparationComponent = evacuAgentPedestrianBase.GetComponentInChildren<BoidInterGroupSeparationComponent>();
        friendGroupBoidBehaviour = (FriendGroupBoidBehaviour)BoidTestsSetupHelper.GetBoidBehaviourStrategyBaseFromEvacuAgentPedestrianBase(evacuAgentPedestrianBase);

        // Spawn the groups
        visibleEvacuAgentBaseGroups = new List<List<EvacuAgentPedestrianBase>>
        {
            SpawnFriendGroupOfEvacuAgentPedestrians(3),
            SpawnFriendGroupOfEvacuAgentPedestrians(2),
            SpawnFriendGroupOfEvacuAgentPedestrians(4),
        };

        // Set positions of all members of each group
        groupPositions = BoidInterGroupSeparationComponentTestsHelper.GetPositionsForGroups();

        for(int index = 0; index < visibleEvacuAgentBaseGroups.Count; index++)
        {
            SetPositions(visibleEvacuAgentBaseGroups[index], groupPositions[index]);
        }

        SetPosition(friendGroupBoidBehaviour, new Vector3(1f, 0f, 5f));

        // Add all members of all groups as a list of visible pedestrians
        visibleNonGroupPedestrians = new List<BoidBehaviourStrategyBase>();

        foreach (List<EvacuAgentPedestrianBase> evacuAgentPedestrianBases in visibleEvacuAgentBaseGroups)
        {
            List<BoidBehaviourStrategyBase> boidBehaviourStrategyBases = BoidTestsSetupHelper.GetBoidBehaviourStrategyBasesFromEvacuAgentPedestrianBases(evacuAgentPedestrianBases);
            visibleNonGroupPedestrians.AddRange(boidBehaviourStrategyBases);
            BoidInterGroupSeparationComponentTestsHelper.SetupBoidBehaviourStrategyBaseGroupCollections(evacuAgentPedestrianBases, boidBehaviourStrategyBases);
        }

        friendGroupBoidBehaviour.NonGroupNeighbours = visibleNonGroupPedestrians;
        friendGroupBoidBehaviour.GroupCollection = evacuAgentPedestrianBase.GroupCollection;

        expectedVelocity = new Vector3(-0.052f, 0f, -0.056f);
    }

    public override void Act()
    {
        actualVelocity = boidInterGroupSeparationComponent.CalculateComponentVelocity(friendGroupBoidBehaviour);
    }

    public override void Assertion()
    {
        AssertTwoVectorsAreEqualWithinTolerance(actualVelocity, expectedVelocity, floatingPointTolerance);
    }

    public void TearDown()
    {
        EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_INTER_GROUP_SEPARATION_WEIGHT = originalWeight;
    }
}

public static class BoidInterGroupSeparationComponentTestsHelper
{
    public static List<List<Vector3>> GetPositionsForGroups()
    {
        return new List<List<Vector3>>
        {
            // Set positions of all members of each group
            new List<Vector3>
            {
                new Vector3(1f, 0f, 2f),
                new Vector3(2f, 0f, 1f),
                new Vector3(3f, 0f, 3f)
            },

            new List<Vector3>
            {
                new Vector3(-1f, 0f, -2f),
                new Vector3(1f, 0f, 3f)
            },

            new List<Vector3>
            {
                new Vector3(0f, 0f, 3f),
                new Vector3(2f, 0f, 2f),
                new Vector3(5f, 0f, -3f),
                new Vector3(2f, 0f, -3f)
            }
        };
    }

    public static void SetupBoidBehaviourStrategyBaseGroupCollections(List<EvacuAgentPedestrianBase> evacuAgentPedestrianBases, List<BoidBehaviourStrategyBase> boidBehaviourStrategyBases)
    {
        for(int index = 0; index < evacuAgentPedestrianBases.Count; index++)
        {
            boidBehaviourStrategyBases[index].GroupCollection = evacuAgentPedestrianBases[index].GroupCollection;
        }
    }
}
