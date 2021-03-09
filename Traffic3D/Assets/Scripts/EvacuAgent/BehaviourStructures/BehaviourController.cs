using System.Collections.Generic;
using UnityEngine;

public class BehaviourController : MonoBehaviour
{
    public List<BehaviourCollection> behaviourCollections = new List<BehaviourCollection>();
    public BehaviourCollection currentBehaviourColection { get; set; }
    public GameObject fieldOfView;

    private void FixedUpdate()
    {
        currentBehaviourColection.PerformBehaviours();
    }
}
