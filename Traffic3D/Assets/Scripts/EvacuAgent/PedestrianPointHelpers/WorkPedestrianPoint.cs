public class WorkPedestrianPoint : BuildingPedestrianPointTypeBase
{
    protected override int timeToWaitLowerBounds => EvacuAgentSceneParamaters.WORK_WAIT_TIME_LOWER_BOUND;
    protected override int timeToWaitUpperBounds => EvacuAgentSceneParamaters.WORK_WAIT_TIME_UPPER_BOUND;

    private void Awake()
    {
        pedestrianPointType = PedestrianPointType.Work;
    }
}
