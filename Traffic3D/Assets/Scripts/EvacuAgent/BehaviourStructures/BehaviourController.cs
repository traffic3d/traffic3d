using System.Collections.Generic;
using UnityEngine;

public class BehaviourController : MonoBehaviour
{
    public List<BehaviourCollection> behaviourCollections = new List<BehaviourCollection>();
    public BehaviourCollection currentBehaviourCollection { get; set; }
    public GameObject fieldOfView;

    private void FixedUpdate()
    {
        currentBehaviourCollection.PerformBehaviours();
    }
}
