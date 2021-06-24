public class WorkerLeaderPedestrianFactory : AbstractEvacuAgentPedestrianFactory
{
    private void Awake()
    {
        numPedestriansToSpawn = EvacuAgentSceneParamaters.NUMBER_OF_WORKER_AGENTS;
    }

    public override EvacuAgentPedestrianBase CreateEvacuAgentPedestrian(Pedestrian pedestrian)
    {
        numPedestriansToSpawn--;
        return CreatePedestrianType(pedestrian, EvacuAgentSceneParamaters.IS_WORKER_HIGHTLIGHT_VISUAL_ENABLED, leaderPedestrianTypePrefab);
    }
}
