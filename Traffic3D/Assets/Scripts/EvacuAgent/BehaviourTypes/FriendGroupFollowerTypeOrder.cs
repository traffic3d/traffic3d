using System;
using System.Collections.Generic;

public class FriendGroupFollowerTypeOrder : BehaviourTypeOrder
{
    private void Awake()
    {
        behaviourTypes = new List<BehaviourType>()
        {
            new GenericEnterLeaveBuildingBehaviourType(),
            new WaitAtDestinationBehaviourType(),
            new FollowerDestinationUpdateBehaviourType(),
            new FriendGroupBoidBehaviourType(),
            new GenericNoNewBehaviourType()
        };
    }

    public override List<BehaviourType> GetBehaviourTypes() => behaviourTypes;

    private class GenericEnterLeaveBuildingBehaviourType : BehaviourType
    {
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override Type GetBehaviourStrategyClass<BehaviourStrategy>() => typeof(GenericEnterLeaveBuildingBehaviour);
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }

    private class GenericNoNewBehaviourType : BehaviourType
    {
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override Type GetBehaviourStrategyClass<BehaviourStrategy>() => typeof(GenericNoNewBehaviour);
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }

    private class FriendGroupBoidBehaviourType : BehaviourType
    {
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override Type GetBehaviourStrategyClass<BehaviourStrategy>() => typeof(FriendGroupBoidBehaviour);
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }

    private class WaitAtDestinationBehaviourType : BehaviourType
    {
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override Type GetBehaviourStrategyClass<BehaviourStrategy>() => typeof(WaitAtDestinationBehaviour);
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }

    private class FollowerDestinationUpdateBehaviourType : BehaviourType
    {
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override Type GetBehaviourStrategyClass<BehaviourStrategy>() => typeof(FollowerDestinationUpdateBehaviour);
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }
}
