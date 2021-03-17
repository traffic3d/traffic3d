/* This behaviour is needed to stop null pointer exceptions being thrown when no new
 * behaviourStrategies are called in a fixedUpdate().
 * Scenarios for this include if the agent is simply walking from one node to another, this is mostly handled
 * by the NavMeshAgent and so no other behaviours may trigger.
 */
public class GenericNoNewBehaviour : BehaviourStrategy
{

    public override bool ShouldTriggerBehaviour()
    {
        return true;
    }

    public override void PerformBehaviour() { }
}
