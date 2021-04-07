using UnityEngine;

public class ShooterPedestrianFactory : AbstractEvacuAgentPedestrianFactory
{
    private void Awake()
    {
        numPedestriansToSpawn = EvacuAgentSceneParamaters.NUMBER_OF_SHOOTER_AGENTS;
    }

    public override EvacuAgentPedestrianBase CreateEvacuAgentPedestrian(Pedestrian pedestrian)
    {
        numPedestriansToSpawn--;
        return CreatePedestrianType(pedestrian, EvacuAgentSceneParamaters.IS_SHOOTER_HIGHTLIGHT_VISUAL_ENABLED, leaderPedestrianTypePrefab);
    }
}
