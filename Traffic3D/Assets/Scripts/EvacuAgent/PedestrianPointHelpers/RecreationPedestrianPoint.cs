public class RecreationPedestrianPoint : BuildingPedestrianPointTypeBase
{
    protected override int timeToWaitLowerBounds => EvacuAgentSceneParamaters.RECREATION_WAIT_TIME_LOWER_BOUND;
    protected override int timeToWaitUpperBounds => EvacuAgentSceneParamaters.RECREATION_WAIT_TIME_UPPER_BOUND;

    private void Awake()
    {
        pedestrianPointType = PedestrianPointType.Recreation;
    }
}
