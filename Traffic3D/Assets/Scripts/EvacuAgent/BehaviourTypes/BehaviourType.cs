using System;

public abstract class BehaviourType
{
    public abstract Type GetBehaviourStrategyClass<T>() where T : BehaviourStrategy;
    public abstract float GetBehaviourStrategyChance();
}
