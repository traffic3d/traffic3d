using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourTypeOrder : MonoBehaviour
{
    protected List<BehaviourType> behaviourTypes;
    public abstract List<BehaviourType> GetBehaviourTypes();
}
