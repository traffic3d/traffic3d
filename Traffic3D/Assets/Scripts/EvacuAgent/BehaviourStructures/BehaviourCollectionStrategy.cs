using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourCollectionStrategy : MonoBehaviour
{
    public List<BehaviourStrategy> behaviours = new List<BehaviourStrategy>();

    public abstract void PerformBehaviours();
}
