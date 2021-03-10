using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourCollection : MonoBehaviour
{
    public List<BehaviourStrategy> behaviours = new List<BehaviourStrategy>();

    [SerializeField]
    private BehaviourStrategy currentBehaviour;

    public void PerformBehaviours()
    {
        foreach(BehaviourStrategy behaviourStrategy in behaviours)
        {
            if (behaviourStrategy.ShouldTriggerBehaviour())
            {
                behaviourStrategy.PerformBehaviour();
                currentBehaviour = behaviourStrategy;
                break;
            }
        }
    }
}
