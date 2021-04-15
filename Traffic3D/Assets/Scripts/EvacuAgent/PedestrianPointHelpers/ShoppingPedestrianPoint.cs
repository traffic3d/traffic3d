public class ShoppingPedestrianPoint : BuildingPedestrianPointTypeBase
{
    protected override int timeToWaitLowerBounds => EvacuAgentSceneParamaters.SHOPPING_WAIT_TIME_LOWER_BOUND;
    protected override int timeToWaitUpperBounds => EvacuAgentSceneParamaters.SHOPPING_WAIT_TIME_UPPER_BOUND;

    private void Awake()
    {
        pedestrianPointType = PedestrianPointType.Shopping;
    }
}
