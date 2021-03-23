using UnityEngine;

public class WorkerPedestrianFactory : AbstractEvacuAgentPedestrianFactory
{
    public override EvacuAgentPedestrianBase CreateEvacuaAgentPedestrian(Pedestrian pedestrian)
    {
        return CreatePedestrianType(pedestrian, EvacuAgentSceneParamaters.IS_WORKER_HIGHTLIGHT_VISUAL_ENABLED, pedestrianTypePrefab);
    }
}
