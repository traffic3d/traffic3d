using System;

public class BehaviourTypeAttribute : Attribute
{
    public string BehaviourStrategyName { get; set; }
    public float BehaviourChance { get; set; }

    public BehaviourTypeAttribute(string behaviourStrategyName, float behaviourChance)
    {
        BehaviourStrategyName = behaviourStrategyName;
        BehaviourChance = behaviourChance;
    }
}
