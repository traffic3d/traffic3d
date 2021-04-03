using System.Collections;
using UnityEngine;

public class TriggerFollowerBoidBehaviour : BehaviourStrategy
{
    private bool shouldTriggerFollowerBoidBehaviour;
    private float boidBehaviourTriggerCoolDownInSeconds;
    private GroupCollection groupCollection;

    private void Start()
    {
        shouldTriggerFollowerBoidBehaviour = true;
        boidBehaviourTriggerCoolDownInSeconds = 0.1f;
        groupCollection = gameObject.GetComponentInParent<GroupCollection>();
    }

    public override bool ShouldTriggerBehaviour()
    {
        return shouldTriggerFollowerBoidBehaviour;
    }

    public override void PerformBehaviour()
    {
        if(groupCollection.GetFollowers().Count > 0)
        {
            //groupCollection.GetFollowers().ForEach(x => x.GetComponentInChildren<GroupFollowerBoidBehaviour>().TriggerFollowerBoidBehaviour());
            //StartCoroutine(TriggerBoidBehaviourCoolDown());
        }
    }

    IEnumerator TriggerBoidBehaviourCoolDown()
    {
        shouldTriggerFollowerBoidBehaviour = false;
        yield return new WaitForSeconds(boidBehaviourTriggerCoolDownInSeconds);
        shouldTriggerFollowerBoidBehaviour = true;
    }
}
