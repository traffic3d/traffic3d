using System.Collections.Generic;

public class WorkerLeaderBehaviourTypeOrder : BehaviourTypeOrder
{
    private void Awake()
    {
        behaviourTypes = new List<BehaviourType>()
        {
            new GenericPathCreationBehaviourType(),
            new GenericEnterLeaveBuildingBehaviourType(),
            new WaitForFollowersBehaviourType(),
            new WaitAtDestinationBehaviourType(),
            new WorkerGroupBoidBehaviourType(),
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

    private class WorkerGroupBoidBehaviourType : BehaviourType
    {
        private readonly string behaviourStrategyName = "WorkerGroupBoidBehaviour";
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override string GetBehaviourStrategyName() => behaviourStrategyName;
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }

    private class WaitAtDestinationBehaviourType : BehaviourType
    {
        private readonly string behaviourStrategyName = "WaitAtDestinationBehaviour";
        private readonly float behaviourStrategyChanceToUse = 1f;

        public override string GetBehaviourStrategyName() => behaviourStrategyName;
        public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
    }
}
