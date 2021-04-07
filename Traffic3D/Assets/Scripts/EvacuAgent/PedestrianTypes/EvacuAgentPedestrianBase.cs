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
    }

    public void AddGroupCollection(GroupCollection groupCollection)
    {
        GroupCollection = groupCollection;
    }

    public void ChangeSpeedToMatchLeader(float leaderSpeed)
    {
        navMeshAgent.speed = leaderSpeed;
    }
}
