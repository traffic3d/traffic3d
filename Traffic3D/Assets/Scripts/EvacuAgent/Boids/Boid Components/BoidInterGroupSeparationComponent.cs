using System.Collections.Generic;
using UnityEngine;

public class BoidInterGroupSeparationComponent : BoidComponentBase
{
    public Dictionary<GroupCollection, List<Vector3>> positionOfVisibleMembersInEachSubGroup { get; private set; }

    private void Awake()
    {
        positionOfVisibleMembersInEachSubGroup = new Dictionary<GroupCollection, List<Vector3>>();
    }

    public override Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        Vector3 velocity = Vector3.zero;

        if (followerBoidBehaviour.NonGroupNeighbours.Count == 0)
            return velocity;

        GetPositionsOfNonGroupPedestriansInAssociatedSubGroups(followerBoidBehaviour.NonGroupNeighbours);
        List<Vector3> groupCentres = FindCentresOfAllVisbleNonGroupPedestrianSubGroups();

        foreach (Vector3 groupCentre in groupCentres)
        {
            float distance = Vector3.Distance(followerBoidBehaviour.transform.position, groupCentre);
            velocity += (groupCentre - followerBoidBehaviour.transform.position).normalized / Mathf.Pow(distance, 2);
        }

        velocity /= followerBoidBehaviour.NonGroupNeighbours.Count;
        velocity *= -1;

        return velocity.normalized * followerBoidBehaviour.InterGroupSeparationWeight;
    }

    public void GetPositionsOfNonGroupPedestriansInAssociatedSubGroups(List<BoidBehaviourStrategyBase> visibleNonGroupPedestrians)
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

    public List<Vector3> FindCentresOfAllVisbleNonGroupPedestrianSubGroups()
    {
        List<Vector3> centres = new List<Vector3>();

        foreach(KeyValuePair<GroupCollection, List<Vector3>> keyValuePair in positionOfVisibleMembersInEachSubGroup)
        {
            Vector3 center = FindCentreOfSingleSubGroup(keyValuePair.Value);
            centres.Add(center);
        }

        return centres;
    }

    public Vector3 FindCentreOfSingleSubGroup(List<Vector3> subGroup)
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
