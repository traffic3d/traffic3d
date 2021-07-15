using UnityEngine;

public abstract class BoidComponentBase : MonoBehaviour
{
    protected abstract bool IsDebuggingOn { get; }
    public abstract Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour);

    public bool DoesVectorContainNaN(Vector3 vector)
    {
        return IsValueNaN(vector[0]) && IsValueNaN(vector[1]) && IsValueNaN(vector[2]);
    }

    private bool IsValueNaN(float value)
    {
        return float.IsNaN(value);
    }
}
