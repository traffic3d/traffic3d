public class WorkerPedestrian : EvacuAgentPedestrianBase
{
    public override void InitialisePedestrian(Pedestrian pedestrian)
    {
        base.InitialisePedestrian(pedestrian);
        gameObject.tag = EvacuAgentSceneParamaters.PEDESTRIAN_TAG;
    }
}
