using UnityEngine;

public abstract class BoidComponentBase : MonoBehaviour
{
    public abstract Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour);

    public bool DoesVectorContainNaN(Vector3 vector)
    {
        for(int index = 0; index < 3; index++)
        {
            if (IsValueNaN(vector[index]))
                return true;
        }

        return false;
    }

    private bool IsValueNaN(float value)
    {
        if (float.IsNaN(value))
            return true;

        return false;
    }
}
