using UnityEngine;

public class WorkerPedestrianFactory : AbstractEvacuAgentPedestrianFactory
{
    public override EvacuAgentPedestrianBase CreateEvacuaAgentPedestrian(Pedestrian pedestrian)
    {
        GameObject workerPedestrianObj = GameObject.Instantiate(pedestrianTypePrefab, pedestrian.transform);
        WorkerPedestrian workerPedestrian = workerPedestrianObj.GetComponent<WorkerPedestrian>();

        workerPedestrian.InitialisePedestrian(pedestrian);
        bool isHighlightEnabled = EvacuAgentSceneParamaters.IS_WORKER_HIGHTLIGHT_VISUAL_ENABLED;
        AddPedestrianHighlighter(pedestrian, workerPedestrian.pedestrianHighlight, isHighlightEnabled);
        AddBehaviourCollection(workerPedestrian.behaviourController, workerPedestrian.behaviourTypeOrder);

        return workerPedestrian;
    }
}
