using System.Collections.Generic;

public class ShooterBehaviourTypeOrder : BehaviourTypeOrder
{
    /* Note that order matters. The closer to 0 in the enum the higher preference it has in the subsumption hierarchy
    *  Meaning it will be iterated over first and potentially chosen over other behaviours
    *  1f here is the chance the behaviour has of being added to a BehaviourCollection (this is not used yet, 09/03/21)
    */
    private void Start()
    {
        behaviourTypes = new List<BehaviourType>()
        {
            new FollowClosestTargetBehaviourType(),
            new CreateWeightedPathOfPedestrianPointsBehaviourType(),
            new MoveToNextDestinationBehaviourtBehaviourType()
        };
    }

    public override List<BehaviourType> GetBehaviourTypes() => behaviourTypes;

    private class FollowClosestTargetBehaviourType : BehaviourType
    {
        private readonly string behaviourStrategyName = "FollowClosestTargetBehaviour";
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override string GetBehaviourStrategyName() => behaviourStrategyName;
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }

    private class CreateWeightedPathOfPedestrianPointsBehaviourType : BehaviourType
    {
        private readonly string behaviourStrategyName = "CreateWeightedPathOfPedestrianPointsBehaviour";
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override string GetBehaviourStrategyName() => behaviourStrategyName;
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }

    private class MoveToNextDestinationBehaviourtBehaviourType : BehaviourType
    {
        private readonly string behaviourStrategyName = "MoveToNextDestinationBehaviour";
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override string GetBehaviourStrategyName() => behaviourStrategyName;
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }
}
