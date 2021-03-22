using UnityEngine;

public abstract class EvacuAgentPedestrianBase : MonoBehaviour
{
    [SerializeField]
    public GameObject behaviourControllerPrefab;

    public GameObject pedestrianHighlight;
    public BehaviourController behaviourController;
    public FieldOfView fieldOfView;
    public PedestrianPointPathCreator PedestrianPointPathCreator;
    public BehaviourTypeOrder behaviourTypeOrder;
    public Pedestrian pedestrian;
    public string Tag { get; protected set; }

    public virtual void InitialisePedestrian(Pedestrian pedestrian)
    {
        this.pedestrian = pedestrian;
        GameObject behaviourControllerPrefab = (GameObject)Resources.Load(EvacuAgentSceneParamaters.BEHAVIOUR_CONTROLLER_PREFAB);
        GameObject behaviourControllerObj = Instantiate(behaviourControllerPrefab, pedestrian.transform);
        behaviourController = behaviourControllerObj.GetComponent<BehaviourController>();
        behaviourController.isUpdateOn = false;
        GameObject fieldOfView = Instantiate(behaviourController.fieldOfView, behaviourController.transform.position, Quaternion.identity);
        fieldOfView.transform.SetParent(pedestrian.transform);
        fieldOfView.GetComponent<MeshRenderer>().enabled = EvacuAgentSceneParamaters.IS_FOV_VISUAL_ENABLED;
    }
}
