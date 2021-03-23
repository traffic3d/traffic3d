using UnityEngine;

public class ShooterPedestrianFactory : AbstractEvacuAgentPedestrianFactory
{
    public override EvacuAgentPedestrianBase CreateEvacuaAgentPedestrian(Pedestrian pedestrian)
    {
        return CreatePedestrianType(pedestrian, EvacuAgentSceneParamaters.IS_SHOOTER_HIGHTLIGHT_VISUAL_ENABLED, pedestrianTypePrefab);
    }
}
