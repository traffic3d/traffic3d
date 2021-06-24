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
        radiusToConsiderForMeetingLocation = 50;
        pedestrianPointTypes = new List<PedestrianPointType>()
        {
            PedestrianPointType.Hospitality,
            PedestrianPointType.Recreation,
            PedestrianPointType.Shopping
        };
    }

    public override List<Vector3> CreatePath()
    {
        List<Vector3> path = new List<Vector3>();

        Vector3 meetingLocationCenterPoint = GetRandomPedestrianPointOfType(PedestrianPointType.Hospitality).GetPointLocation();
        Vector3 meetingLocation = chooseLocationOnNavmesh.GetRandomPointOnNavMesh(meetingLocationCenterPoint, radiusToConsiderForMeetingLocation);
        path.Add(meetingLocation);

        Vector3 lastPathLocation = meetingLocation;
        int index = 0;

        while(index < sizeOfPath)
        {
            PedestrianPointType pedestrianPointType = pedestrianPointTypes[Random.Range(0, pedestrianPointTypes.Count)];
            Vector3 currentPedestrianPointDestination = GetRandomPedestrianPointOfType(pedestrianPointType).GetPointLocation();

            // Ensures that the same destination is not in adjacent elements of path
            if (!currentPedestrianPointDestination.Equals(lastPathLocation))
            {
                path.Add(GetRandomPedestrianPointOfType(pedestrianPointType).GetPointLocation());
                lastPathLocation = currentPedestrianPointDestination;
                index++;
            }
        }

        return path;
    }
}
