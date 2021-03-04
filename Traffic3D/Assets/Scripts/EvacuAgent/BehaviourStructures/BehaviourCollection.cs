using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourCollection : BehaviourCollectionStrategy
{
    [SerializeField]
    private BehaviourStrategy currentBehaviour;

    public override void PerformBehaviours()
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
