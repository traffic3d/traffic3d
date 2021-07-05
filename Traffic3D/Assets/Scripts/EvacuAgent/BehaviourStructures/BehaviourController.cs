using System.Collections.Generic;
using UnityEngine;

public class BehaviourController : MonoBehaviour
{
    public List<BehaviourCollection> behaviourCollections = new List<BehaviourCollection>();
    public BehaviourCollection currentBehaviourCollection { get; set; }
    public GameObject fieldOfView;
    public bool isUpdateOn { get; set; } = true;

    private void FixedUpdate()
    {
        if(isUpdateOn == true)
        {
            currentBehaviourCollection.PerformBehaviours();
        }
    }
}
