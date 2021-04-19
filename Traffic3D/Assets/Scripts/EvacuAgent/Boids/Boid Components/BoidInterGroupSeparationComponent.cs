using System.Collections.Generic;
using UnityEngine;

public class BoidInterGroupSeparationComponent : BoidComponentBase
{
    public Dictionary<GroupCollection, List<Vector3>> positionOfVisibleMembersInEachSubGroup { get; private set; }

    protected override bool IsDebuggingOn => false;
    private Vector3 velocityDebugCache;
    private Vector3 groupDestinationDebugCache;
    private List<Vector3> groupCentres;
    private List<Vector3> directionsFromGroupCentresToTargetDebugCache;

    private void Awake()
    {
        positionOfVisibleMembersInEachSubGroup = new Dictionary<GroupCollection, List<Vector3>>();
        groupCentres = new List<Vector3>();
        directionsFromGroupCentresToTargetDebugCache = new List<Vector3>();
    }

    public override Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        Vector3 velocity = Vector3.zero;
        velocityDebugCache = velocity;
        groupDestinationDebugCache = followerBoidBehaviour.GroupCollection.GroupDestination;


        positionOfVisibleMembersInEachSubGroup.Clear();
        groupCentres.Clear();
        directionsFromGroupCentresToTargetDebugCache.Clear();
        if (followerBoidBehaviour.NonGroupNeighbours.Count == 0)
            return velocity;

        GetPositionsOfNonGroupPedestriansInAssociatedSubGroups(followerBoidBehaviour.NonGroupNeighbours);
        groupCentres = FindCentresOfAllVisbleNonGroupPedestrianSubGroups();

        foreach (Vector3 groupCentre in groupCentres)
        {
            float distance = Vector3.Distance(followerBoidBehaviour.transform.position, groupCentre);
            Vector3 directionFromCentreToTarget = (followerBoidBehaviour.GroupCollection.GroupDestination - groupCentre).normalized;
            velocity += directionFromCentreToTarget / Mathf.Pow(distance, 2);
            directionsFromGroupCentresToTargetDebugCache.Add(directionFromCentreToTarget);
        }

        velocityDebugCache = velocity;

        return velocity * followerBoidBehaviour.InterGroupSeparationWeight;
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

    private void OnDrawGizmos()
    {
        if (!IsDebuggingOn)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, (velocityDebugCache - transform.position).normalized * 10f);
        Gizmos.color = Color.red;
        //groupCentres.ForEach(x => Gizmos.DrawLine(transform.position, x));
        directionsFromGroupCentresToTargetDebugCache.ForEach(x => Gizmos.DrawRay(transform.position, x * 15f));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, groupDestinationDebugCache);
        Gizmos.color = Color.white;
    }
}
