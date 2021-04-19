using UnityEngine;

public class WorkerGroupBoidBehaviour : BoidBehaviourStrategyBase
{
    public override float CohesionWeight => EvacuAgentSceneParamaters.WORKER_GROUP_BOID_COHESION_WEIGHT;
    public override float SeparationWeight => EvacuAgentSceneParamaters.WORKER_GROUP_BOID_SEPARATION_WEIGHT;
    public override float TargetSeekingWeight => EvacuAgentSceneParamaters.WORKER_GROUP_BOID_TARGET_SEEKING_WEIGHT;
    public override float InterGroupSeparationWeight => EvacuAgentSceneParamaters.WORKER_GROUP_BOID_INTER_GROUP_SEPARATION_WEIGHT;

    protected override bool IsDebuggingOn => false;

    void Start()
    {
        base.Start();
        shouldUpdateBoid = true;
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
