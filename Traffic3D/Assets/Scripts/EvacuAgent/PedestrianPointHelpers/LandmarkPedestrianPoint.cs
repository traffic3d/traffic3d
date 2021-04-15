public class LandmarkPedestrianPoint : BuildingPedestrianPointTypeBase
{
    protected override int timeToWaitLowerBounds => EvacuAgentSceneParamaters.LANDMARK_WAIT_TIME_LOWER_BOUND;
    protected override int timeToWaitUpperBounds => EvacuAgentSceneParamaters.LANDMARK_WAIT_TIME_UPPER_BOUND;

    private void Awake()
    {
        pedestrianPointType = PedestrianPointType.Landmark;
    }
}
