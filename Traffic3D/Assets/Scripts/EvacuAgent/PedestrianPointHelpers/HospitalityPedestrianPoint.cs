public class HospitalityPedestrianPoint : BuildingPedestrianPointTypeBase
{
    protected override int timeToWaitLowerBounds => EvacuAgentSceneParamaters.HOSPITALITY_WAIT_TIME_LOWER_BOUND;
    protected override int timeToWaitUpperBounds => EvacuAgentSceneParamaters.HOSPITALITY_WAIT_TIME_UPPER_BOUND;

    private void Awake()
    {
        pedestrianPointType = PedestrianPointType.Hospitality;
    }
}
