using UnityEngine;
using UnityEngine.AI;

public class PedestrianPointBuildingEnterence : MonoBehaviour
{
    private PedestrianPoint pedestrianPoint;
    private int timeToWaitLowerBounds;
    private int timeToWaitUpperBounds;

    private void Start()
    {
        pedestrianPoint = GetComponent<PedestrianPoint>();
        GetSecondsToWait();
    }

    void OnTriggerEnter(Collider collider)
    {
        GenericEnterLeaveBuildingBehaviour genericEnterLeaveBuildingBehaviour = collider.transform.GetComponentInChildren<GenericEnterLeaveBuildingBehaviour>();
        if (genericEnterLeaveBuildingBehaviour != null)
        {
            NavMeshAgent pedestrianNavMeshAgent = genericEnterLeaveBuildingBehaviour.GetComponentInParent<NavMeshAgent>();

            if (Vector3.Distance(pedestrianNavMeshAgent.destination, transform.position) < 2f)
            {
                int secondsToWait = Random.Range(timeToWaitLowerBounds, timeToWaitUpperBounds);
                genericEnterLeaveBuildingBehaviour.SetBuildingStayParamaters(secondsToWait, true);
            }
        }
    }

    private void GetSecondsToWait()
    {
        if(pedestrianPoint.PedestrianPointType == PedestrianPointType.Hospitality)
        {
            timeToWaitLowerBounds = EvacuAgentSceneParamaters.HOSPITALITY_WAIT_TIME_LOWER_BOUND;
            timeToWaitUpperBounds = EvacuAgentSceneParamaters.HOSPITALITY_WAIT_TIME_UPPER_BOUND;
        }
        else if(pedestrianPoint.PedestrianPointType == PedestrianPointType.Work)
        {
            timeToWaitLowerBounds = EvacuAgentSceneParamaters.WORK_WAIT_TIME_LOWER_BOUND;
            timeToWaitUpperBounds = EvacuAgentSceneParamaters.WORK_WAIT_TIME_UPPER_BOUND;
        }
    }
}
