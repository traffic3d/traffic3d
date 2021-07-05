using UnityEngine;

public abstract class BehaviourStrategy : MonoBehaviour
{
    public abstract bool ShouldTriggerBehaviour();
    public abstract void PerformBehaviour();
}
