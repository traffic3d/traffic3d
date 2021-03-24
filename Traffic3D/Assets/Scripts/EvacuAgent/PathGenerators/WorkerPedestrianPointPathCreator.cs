using System.Collections.Generic;

public class WorkerPedestrianPointPathCreator : NonShooterPedestrianPointPathCreator
{
    public override List<PedestrianPoint> CreatePath()
    {
        List<PedestrianPoint> path = new List<PedestrianPoint>();

        if (UnityEngine.Random.value < EvacuAgentSceneParamaters.WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE)
        {
            path.Add(GetRandomPedestrianPointOfType(PedestrianPointType.Hospitality));
        }

        path.Add(GetRandomPedestrianPointOfType(PedestrianPointType.Work));

        return path;
    }
}
