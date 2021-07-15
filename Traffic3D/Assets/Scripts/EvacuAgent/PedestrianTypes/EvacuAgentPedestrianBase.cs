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
    public List<EvacuAgentPedestrianBase> visibleGroupMembers;
    public List<EvacuAgentPedestrianBase> visibleNonGroupMembers;

    private float groupMemberAwarenessDistance;

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
        visibleGroupMembers = new List<EvacuAgentPedestrianBase>();
        visibleNonGroupMembers = new List<EvacuAgentPedestrianBase>();
        groupMemberAwarenessDistance = 15f;
    }

    public void AddGroupCollection(GroupCollection groupCollection)
    {
        GroupCollection = groupCollection;
    }

    public void ChangeSpeedToMatchLeader(float leaderSpeed)
    {
        navMeshAgent.speed = leaderSpeed;
    }

    public void IsPedestrianMovementStopped(bool isMovementStopped)
    {
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.isStopped = isMovementStopped;
        boidManager.isBoidMovementStopped = isMovementStopped;
    }

    public List<EvacuAgentPedestrianBase> GetVisibleGroupMembers()
    {
        UpdateVisibleGroupAndNonGroupPedestrians();

        return visibleGroupMembers;
    }

    public List<EvacuAgentPedestrianBase> GetVisibleNonGroupMembers()
    {
        UpdateVisibleGroupAndNonGroupPedestrians();

        return visibleNonGroupMembers;
    }

    private void UpdateVisibleGroupAndNonGroupPedestrians()
    {
        visibleGroupMembers.Clear();
        visibleNonGroupMembers.Clear();

        foreach (Pedestrian pedestrian in fieldOfView.allVisiblePedestrians)
        {
            EvacuAgentPedestrianBase evacuAgentPedestrian = pedestrian.GetComponentInChildren<EvacuAgentPedestrianBase>();

            if (pedestrian.transform.root == transform.root)
                continue;

            if (GroupCollection.GetGroupMembers().Contains(evacuAgentPedestrian))
            {
                visibleGroupMembers.Add(evacuAgentPedestrian);
            }
            else
            {
                visibleNonGroupMembers.Add(evacuAgentPedestrian);
            }
        }
    }
}
