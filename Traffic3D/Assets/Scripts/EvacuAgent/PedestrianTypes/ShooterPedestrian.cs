public class ShooterPedestrian : EvacuAgentPedestrianBase
{
    public override void InitialisePedestrian(Pedestrian pedestrian)
    {
        base.InitialisePedestrian(pedestrian);
        pedestrian.isShooterAgent = true;
        gameObject.tag = EvacuAgentSceneParamaters.SHOOTER_TAG;
    }
}
