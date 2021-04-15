using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EvacuAgentPedestrianBase : MonoBehaviour
{
    [SerializeField]
    public GameObject behaviourControllerPrefab;

    [SerializeField]
    public GroupCollection GroupCollection;

    public GameObject pedestrianHighlight;
    public BehaviourController behaviourController;
    public FieldOfView fieldOfView;
    public PedestrianPointPathCreator PedestrianPointPathCreator;
    public BehaviourTypeOrder behaviourTypeOrder;
    public Pedestrian pedestrian;
    public NavMeshAgent navMeshAgent;
    public BoidManager boidManager;
    public string Tag { get; protected set; }

    public virtual void InitialisePedestrian(Pedestrian pedestrian)
    {
        this.pedestrian = pedestrian;
        GameObject behaviourControllerPrefab = (GameObject)Resources.Load(EvacuAgentSceneParamaters.BEHAVIOUR_CONTROLLER_PREFAB);
        GameObject behaviourControllerObj = Instantiate(behaviourControllerPrefab, pedestrian.transform);
        behaviourController = behaviourControllerObj.GetComponent<BehaviourController>();
        behaviourController.isUpdateOn = false;
        GameObject fieldOfViewObj = Instantiate(behaviourController.fieldOfView, behaviourController.transform.position, Quaternion.identity);
        fieldOfViewObj.transform.SetParent(pedestrian.transform);
        fieldOfViewObj.GetComponent<MeshRenderer>().enabled = EvacuAgentSceneParamaters.IS_FOV_VISUAL_ENABLED;
        fieldOfView = fieldOfViewObj.GetComponent<FieldOfView>();
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
        boidManager = GetComponent<BoidManager>();
    }

    public void AddGroupCollection(GroupCollection groupCollection)
    {
        GroupCollection = groupCollection;
    }

    public void ChangeSpeedToMatchLeader(float leaderSpeed)
    {
        navMeshAgent.speed = leaderSpeed;
    }

    public List<EvacuAgentPedestrianBase> GetVisibleGroupMemebers()
    {
        List<EvacuAgentPedestrianBase> visibleGroupMembers = new List<EvacuAgentPedestrianBase>();

        foreach (Pedestrian pedestrian in fieldOfView.visiblePedestrians)
        {
            EvacuAgentPedestrianBase evacuAgentPedestrian = pedestrian.GetComponentInChildren<EvacuAgentPedestrianBase>();

            if (pedestrian.transform.root == transform.root)
                continue;

            if (GroupCollection.GetGroupMembers().Contains(evacuAgentPedestrian))
            {
                visibleGroupMembers.Add(evacuAgentPedestrian);
            }
        }

        return visibleGroupMembers;
    }

    public void IsPedestrianMovementStopped(bool isMovementActive)
    {
        navMeshAgent.isStopped = isMovementActive;
        boidManager.isBoidMovementStopped = isMovementActive;
    }
}
