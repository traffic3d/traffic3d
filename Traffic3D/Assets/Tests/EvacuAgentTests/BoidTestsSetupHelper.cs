using NUnit.Framework;
using System;
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

    public static BoidBehaviourStrategyBase GetBoidBehaviourStrategyBaseFromEvacuAgentPedestrianBase(EvacuAgentPedestrianBase evacuAgentPedestrianBase)
    {
        BoidBehaviourStrategyBase boidBehaviourStrategyBase = evacuAgentPedestrianBase.GetComponentInChildren<BoidBehaviourStrategyBase>();
        return boidBehaviourStrategyBase;
    }

    public static List<BoidBehaviourStrategyBase> GetBoidBehaviourStrategyBasesFromEvacuAgentPedestrianBases(List<EvacuAgentPedestrianBase> evacuAgentPedestrianBases)
    {
        List<BoidBehaviourStrategyBase> boidBehaviourStrategyBases = new List<BoidBehaviourStrategyBase>();

        foreach (EvacuAgentPedestrianBase evacuAgentPedestrianBase in evacuAgentPedestrianBases)
        {
            boidBehaviourStrategyBases.Add(GetBoidBehaviourStrategyBaseFromEvacuAgentPedestrianBase(evacuAgentPedestrianBase));
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

    public static void SetNeighbourCentre(BoidBehaviourStrategyBase boidBehaviourStrategyBase, Vector3 neighbourCentre)
    {
        boidBehaviourStrategyBase.NeighbourCenter = neighbourCentre;
    }
}
