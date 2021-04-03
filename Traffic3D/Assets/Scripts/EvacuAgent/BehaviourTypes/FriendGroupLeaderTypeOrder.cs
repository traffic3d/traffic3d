using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendGroupLeaderTypeOrder : BehaviourTypeOrder
{
    private void Awake()
    {
        behaviourTypes = new List<BehaviourType>()
        {
            new GenericPathCreationBehaviourType(),
            //new UpdateFollowerDestinationsBehaviourType(),
            //new TriggerFollowerBoidBehaviourType(),
            new GenericEnterLeaveBuildingBehaviourType(),
            new WaitForFollowersBehaviourType(),
            new GenericMoveToNextDestinationBehaviourType(),
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

    private class GenericPathCreationBehaviourType : BehaviourType
    {
        private readonly string behaviourStrategyName = "GenericPathCreationBehaviour";
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override string GetBehaviourStrategyName() => behaviourStrategyName;
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }

    private class GenericMoveToNextDestinationBehaviourType : BehaviourType
    {
        private readonly string behaviourStrategyName = "GenericMoveToNextDestinationBehaviour";
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

    private class WaitForFollowersBehaviourType : BehaviourType
    {
        private readonly string behaviourStrategyName = "WaitForFollowersBehaviour";
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override string GetBehaviourStrategyName() => behaviourStrategyName;
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }

    private class UpdateFollowerDestinationsBehaviourType : BehaviourType
    {
        private readonly string behaviourStrategyName = "UpdateFollowerDestinationsBehaviour";
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override string GetBehaviourStrategyName() => behaviourStrategyName;
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }

    private class TriggerFollowerBoidBehaviourType : BehaviourType
    {
        private readonly string behaviourStrategyName = "TriggerFollowerBoidBehaviour";
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override string GetBehaviourStrategyName() => behaviourStrategyName;
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }
}
