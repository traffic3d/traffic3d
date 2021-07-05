using System.Collections.Generic;
using UnityEngine;

public class WorkerPedestrianPointPathCreator : NonShooterPedestrianPointPathCreator
{
    public override List<Vector3> CreatePath()
    {
        List<Vector3> path = new List<Vector3>();

        if (UnityEngine.Random.value < EvacuAgentSceneParamaters.WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE)
        {
            path.Add(GetRandomPedestrianPointOfType(PedestrianPointType.Hospitality).GetPointLocation());
        }

        path.Add(GetRandomPedestrianPointOfType(PedestrianPointType.Work).GetPointLocation());

        return path;
    }
}
