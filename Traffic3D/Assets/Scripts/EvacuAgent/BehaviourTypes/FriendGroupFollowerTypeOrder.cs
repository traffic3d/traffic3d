using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendGroupFollowerTypeOrder : BehaviourTypeOrder
{
    private void Awake()
    {
        behaviourTypes = new List<BehaviourType>()
        {
            new GenericEnterLeaveBuildingBehaviourType(),
            //new FollowerDestinationUpdateBehaviourType(),
            //new GroupFollowerBoidBehaviourType(),
            new FriendGroupBoidBehaviourType(),
            new GenericNoNewBehaviourType()
        };
    }

    public override List<BehaviourType> GetBehaviourTypes() => behaviourTypes;

    private class GenericEnterLeaveBuildingBehaviourType : BehaviourType
    {
        private readonly string behaviourStrategyName = "GenericEnterLeaveBuildingBehaviour";
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override string GetBehaviourStrategyName() => behaviourStrategyName;
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }

    private class FollowerDestinationUpdateBehaviourType : BehaviourType
    {
        private readonly string behaviourStrategyName = "FollowerDestinationUpdateBehaviour";
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override string GetBehaviourStrategyName() => behaviourStrategyName;
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }

    private class GenericNoNewBehaviourType : BehaviourType
    {
        private readonly string behaviourStrategyName = "GenericNoNewBehaviour";
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override string GetBehaviourStrategyName() => behaviourStrategyName;
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }

    private class GroupFollowerBoidBehaviourType : BehaviourType
    {
        private readonly string behaviourStrategyName = "GroupFollowerBoidBehaviour";
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override string GetBehaviourStrategyName() => behaviourStrategyName;
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }

    private class FriendGroupBoidBehaviourType : BehaviourType
    {
        private readonly string behaviourStrategyName = "FriendGroupBoidBehaviour";
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override string GetBehaviourStrategyName() => behaviourStrategyName;
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }
}
