using System.Collections.Generic;
using UnityEngine;

public class FriendGroupLeaderPathCreator : NonShooterPedestrianPointPathCreator
{
    private ChooseLocationOnNavmesh chooseLocationOnNavmesh;
    private int sizeOfPath;
    private float radiusToConsiderForMeetingLocation;
    List<PedestrianPointType> pedestrianPointTypes;

    private void Awake()
    {
        chooseLocationOnNavmesh = gameObject.AddComponent<ChooseLocationOnNavmesh>();
        sizeOfPath = 4;
        radiusToConsiderForMeetingLocation = 200f;
        pedestrianPointTypes = new List<PedestrianPointType>()
        {
            PedestrianPointType.Hospitality
        };
    }

    public override List<Vector3> CreatePath()
    {
        List<Vector3> path = new List<Vector3>();

        Vector3 meetingLocationCenterPoint = GetRandomPedestrianPointOfType(PedestrianPointType.Hospitality).GetPointLocation();
        Vector3 meetingLocation = chooseLocationOnNavmesh.GetRandomPointOnNavMesh(meetingLocationCenterPoint, radiusToConsiderForMeetingLocation);
        path.Add(meetingLocation);

        for (int index = 0; index < sizeOfPath; index++)
        {
            PedestrianPointType pedestrianPointType = pedestrianPointTypes[Random.Range(0, pedestrianPointTypes.Count)];
            path.Add(GetRandomPedestrianPointOfType(pedestrianPointType).GetPointLocation());
        }

        return path;
    }
}
