
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoidTestsSetupHelper
{
    public static void PopulateBoidBehaviourListFromEvacuAgentPedestrianList(List<EvacuAgentPedestrianBase> evacuAgentPedestrianBases, List<BoidBehaviourStrategyBase> boidBehaviourStrategyBases)
    {
        foreach (EvacuAgentPedestrianBase evacuAgentPedestrianBase in evacuAgentPedestrianBases)
        {
            boidBehaviourStrategyBases.Add(evacuAgentPedestrianBase.GetComponentInChildren<BoidBehaviourStrategyBase>());
        }
    }

    public static List<Pedestrian> CombineEvacuAgentCollectionsIntoPedestrianList(List<List<EvacuAgentPedestrianBase>> evacuAgentPedestrianBases)
    {
        List<Pedestrian> pedestrians = new List<Pedestrian>();

        foreach (List<EvacuAgentPedestrianBase> evacuAgentPedestrianBasesList in evacuAgentPedestrianBases)
        {
            foreach (EvacuAgentPedestrianBase evacuAgentPedestrianBase in evacuAgentPedestrianBasesList)
            {
                Pedestrian pedestrian = evacuAgentPedestrianBase.GetComponentInParent<Pedestrian>();
                pedestrians.Add(pedestrian);
            }
        }

        return pedestrians;
    }

    public static List<BoidBehaviourStrategyBase> GetBoidBehaviourStrategyBasesFromEvacuAgentPedestrianBases(List<EvacuAgentPedestrianBase> evacuAgentPedestrianBases)
    {
        List<BoidBehaviourStrategyBase> boidBehaviourStrategyBases = new List<BoidBehaviourStrategyBase>();

        foreach (EvacuAgentPedestrianBase evacuAgentPedestrianBase in evacuAgentPedestrianBases)
        {
            boidBehaviourStrategyBases.Add(evacuAgentPedestrianBase.GetComponentInChildren<BoidBehaviourStrategyBase>());
        }

        return boidBehaviourStrategyBases;
    }

    public static KeyValuePair<BoidBehaviourStrategyBase, List<BoidBehaviourStrategyBase>> SeparateLeaderAndGroupMembers(List<BoidBehaviourStrategyBase> boidBehaviourStrategyBases, BoidBehaviourStrategyBase leader)
    {
        List<BoidBehaviourStrategyBase> nonLeaderMembers = new List<BoidBehaviourStrategyBase>();

        foreach (BoidBehaviourStrategyBase boidBehaviourStrategyBase in boidBehaviourStrategyBases)
        {
            if (!boidBehaviourStrategyBase.Equals(leader))
            {
                nonLeaderMembers.Add(boidBehaviourStrategyBase);
            }
        }

        KeyValuePair<BoidBehaviourStrategyBase, List<BoidBehaviourStrategyBase>> leaderAndGroupMembers = new KeyValuePair<BoidBehaviourStrategyBase, List<BoidBehaviourStrategyBase>>(leader, nonLeaderMembers);

        return leaderAndGroupMembers;
    }

    public static void SetPosition<T>(T component, Vector3 position) where T : Component
    {
        component.transform.position = position;
    }

    public static void SetPositions<T>(List<T> components, List<Vector3> positions) where T : Component
    {
        for (int index = 0; index < components.Count; index++)
        {
            components[index].gameObject.transform.position = positions[index];
        }
    }

    public static void SetNeighbourCentre(BoidBehaviourStrategyBase boidBehaviourStrategyBase, Vector3 neighbourCentre)
    {
        boidBehaviourStrategyBase.NeighbourCenter = neighbourCentre;
    }

    public static void AssertTwoVectorsAreEqualWithinTolerance(Vector3 actualVector, Vector3 expectedVector, float tolerance)
    {
        try
        {
            Assert.That(actualVector.x, Is.EqualTo(expectedVector.x).Within(tolerance));
        }
        catch(Exception e)
        {
            Debug.Log($"AssertTwoVectorsAreEqualWithinTolerance - Failure in X component. Message: {e}");
            MakeTestFailWhenExceptionIsCaught();
        }

        try
        {
            Assert.That(actualVector.y, Is.EqualTo(expectedVector.y).Within(tolerance));
        }
        catch (Exception e)
        {
            Debug.Log($"AssertTwoVectorsAreEqualWithinTolerance - Failure in Y component. Message: {e}");
            MakeTestFailWhenExceptionIsCaught();
        }

        try
        {
            Assert.That(actualVector.z, Is.EqualTo(expectedVector.z).Within(tolerance));
        }
        catch (Exception e)
        {
            Debug.Log($"AssertTwoVectorsAreEqualWithinTolerance - Failure in Z component. Message: {e}");
            MakeTestFailWhenExceptionIsCaught();
        }
    }

    // While the Try Catch in AssertTwoVectorsAreEqualWithinTolerance is useful for degubbing it causes failing tests to appear as passing so this is necessary for now
    private static void MakeTestFailWhenExceptionIsCaught() => Assert.IsTrue(false);
}
