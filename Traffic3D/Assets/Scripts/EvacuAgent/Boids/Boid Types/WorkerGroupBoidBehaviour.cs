using UnityEngine;

public class WorkerGroupBoidBehaviour : BoidBehaviourStrategyBase
{
    public override float CohesionWeight => EvacuAgentSceneParamaters.WORKER_GROUP_BOID_COHESION_WEIGHT;
    public override float SeparationWeight => EvacuAgentSceneParamaters.WORKER_GROUP_BOID_SEPARATION_WEIGHT;
    public override float TargetSeekingWeight => EvacuAgentSceneParamaters.WORKER_GROUP_BOID_TARGET_SEEKING_WEIGHT;

    void Start()
    {
        base.Start();
        shouldUpdateBoid = true;
        isDebuggingOn = true;
    }

    public override void PerformBehaviour()
    {
        CalculateNewVelocity();
    }

    public override bool ShouldTriggerBehaviour()
    {
        return shouldUpdateBoid;
    }
}
