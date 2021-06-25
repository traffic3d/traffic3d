using System.Collections.Generic;
using UnityEngine;

public class BoidInterGroupSeparationComponent : BoidComponentBase
{
    private Dictionary<GroupCollection, List<Vector3>> positionOfVisibleMembersInEachSubGroup;

    private void Start()
    {
        positionOfVisibleMembersInEachSubGroup = new Dictionary<GroupCollection, List<Vector3>>();
    }

    public override Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        Vector3 velocity = Vector3.zero;

        return velocity;

        if (followerBoidBehaviour.NonGroupNeighbours.Count == 0)
            return velocity;

        GetPositionsOfNonGroupPedestriansInAssociatedSubGroups(followerBoidBehaviour.NonGroupNeighbours);
        List<Vector3> gorupCentres = FindCentresOfAllVisbleNonGroupPedestrianSubGroups();

        foreach (Vector3 groupCentre in gorupCentres)
        {
            float distance = Vector3.Distance(transform.position, groupCentre);
            velocity += (groupCentre - transform.position).normalized / Mathf.Pow(distance, 2);
        }

        velocity /= followerBoidBehaviour.NonGroupNeighbours.Count;
        velocity *= -1;

        float weight = 0.0008f;
        return velocity.normalized * weight;
    }

    private void GetPositionsOfNonGroupPedestriansInAssociatedSubGroups(List<BoidBehaviourStrategyBase> visibleNonGroupPedestrians)
    {
        foreach (BoidBehaviourStrategyBase neighbour in visibleNonGroupPedestrians)
        {
            if (!positionOfVisibleMembersInEachSubGroup.ContainsKey(neighbour.GroupCollection))
            {
                positionOfVisibleMembersInEachSubGroup.Add(neighbour.GroupCollection, new List<Vector3> { neighbour.transform.position });
            }
            else
            {
                positionOfVisibleMembersInEachSubGroup[neighbour.GroupCollection].Add(neighbour.transform.position);
            }
        }
    }

    private List<Vector3> FindCentresOfAllVisbleNonGroupPedestrianSubGroups()
    {
        List<Vector3> centres = new List<Vector3>();

        foreach(KeyValuePair<GroupCollection, List<Vector3>> keyValuePair in positionOfVisibleMembersInEachSubGroup)
        {
            Vector3 center = FindCentreOfSingleSubGroup(keyValuePair.Value);
            centres.Add(center);
        }

        return centres;
    }

    private Vector3 FindCentreOfSingleSubGroup(List<Vector3> subGroup)
    {
        Vector3 centre = Vector3.zero;

        foreach(Vector3 position in subGroup)
        {
            centre += position;
        }

        centre /= subGroup.Count;

        return centre;
    }
}
