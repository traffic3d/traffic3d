using UnityEngine;

public class ShooterPedestrianFactory : AbstractEvacuAgentPedestrianFactory
{
    public override EvacuAgentPedestrianBase CreateEvacuaAgentPedestrian(Pedestrian pedestrian)
    {
        GameObject shooterPedestrianObj = GameObject.Instantiate(pedestrianTypePrefab, pedestrian.transform);
        ShooterPedestrian shooterPedestrian = shooterPedestrianObj.GetComponent<ShooterPedestrian>();

        shooterPedestrian.InitialisePedestrian(pedestrian);
        bool isHighlightEnabled = EvacuAgentSceneParamaters.IS_SHOOTER_HIGHTLIGHT_VISUAL_ENABLED;
        AddPedestrianHighlighter(pedestrian, shooterPedestrian.pedestrianHighlight, isHighlightEnabled);
        AddBehaviourCollection(shooterPedestrian.behaviourController, shooterPedestrian.behaviourTypeOrder);

        return shooterPedestrian;
    }
}
