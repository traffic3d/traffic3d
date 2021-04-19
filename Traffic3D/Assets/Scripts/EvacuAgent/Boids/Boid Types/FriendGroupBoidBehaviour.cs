using UnityEngine;

public class FriendGroupBoidBehaviour : BoidBehaviourStrategyBase
{
    public override float CohesionWeight => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_COHESION_WEIGHT;
    public override float SeparationWeight => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_SEPARATION_WEIGHT;
    public override float TargetSeekingWeight => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_TARGET_SEEKING_WEIGHT;
    public override float InterGroupSeparationWeight => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_INTER_GROUP_SEPARATION_WEIGHT;

    protected override bool IsDebuggingOn => false;

    void Start()
    {
        base.Start();
        shouldUpdateBoid = true;
        isDebuggingOn = false;
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
