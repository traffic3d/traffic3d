using UnityEngine;

public class PedestrianBehaviourFactory : MonoBehaviour
{
    [SerializeField]
    private BehaviourCollectionFactory shooterBehaviourCollectionFactory;

    [SerializeField]
    private GameObject fieldOfViewPrefab;

    [SerializeField]
    private GameObject shooterHighlightPrefab;

    [SerializeField]
    private GameObject behaviourControllerPrefab;

    private ShooterBehaviourTypeOrder shooterBehaviourTypeOrder;
    private WorkerBehaviourTypeOrder workerBehaviourTypeOrder;
    private int numberOfShooterAgentsSpawned = 0;
    private int numberOfWorkerAgentsSpawned = 0;

    private void Start()
    {
        shooterBehaviourTypeOrder = GetComponent<ShooterBehaviourTypeOrder>();
        workerBehaviourTypeOrder = GetComponent<WorkerBehaviourTypeOrder>();
    }

    public void AddEvacuAgentBehaviour(Pedestrian pedestrian)
    {
        GameObject behaviourControllerInstance = Instantiate(behaviourControllerPrefab, pedestrian.transform);
        BehaviourController behaviourController = behaviourControllerInstance.GetComponent<BehaviourController>();

        if (pedestrian.GetComponentInChildren<FieldOfView>() == null)
        {
            GameObject fieldOfView = Instantiate(behaviourController.fieldOfView, behaviourControllerInstance.transform.position, Quaternion.identity);
            fieldOfView.transform.SetParent(pedestrian.transform);
            fieldOfView.GetComponent<MeshRenderer>().enabled = EvacuAgentSceneParamaters.IS_FOV_VISUAL_ENABLED;
        }

        if(numberOfShooterAgentsSpawned < EvacuAgentSceneParamaters.NUMBER_OF_SHOOTER_AGENTS)
        {
            numberOfShooterAgentsSpawned++;
            pedestrian.isShooterAgent = true;
            AddTag(pedestrian, EvacuAgentSceneParamaters.SHOOTER_TAG);
            AddPedestrianHighlighter(pedestrian, shooterHighlightPrefab, EvacuAgentSceneParamaters.IS_SHOOTER_HIGHTLIGHT_VISUAL_ENABLED);
            AddBehaviourCollection(behaviourController, shooterBehaviourTypeOrder);
            pedestrian.pedestrianType = PedestrianType.Shooter;
        }
        else if (numberOfWorkerAgentsSpawned < EvacuAgentSceneParamaters.NUMBER_OF_WORKER_AGENTS)
        {
            numberOfWorkerAgentsSpawned++;
            AddBehaviourCollection(behaviourController, workerBehaviourTypeOrder);
            pedestrian.pedestrianType = PedestrianType.Worker;
        }
    }

    private void AddTag(Pedestrian pedestrian, string tag)
    {
        pedestrian.tag = tag;
    }

    private void AddPedestrianHighlighter(Pedestrian pedestrian, GameObject prefab, bool isHighlightEnabled)
    {
        GameObject highlight = Instantiate(prefab, pedestrian.transform.position, Quaternion.identity);
        highlight.transform.SetParent(pedestrian.transform);
        highlight.GetComponent<MeshRenderer>().enabled = isHighlightEnabled;
    }

    private void AddBehaviourCollection(BehaviourController behaviourController, BehaviourTypeOrder behaviourTypeOrder)
    {
        BehaviourCollection behaviourCollection = shooterBehaviourCollectionFactory.GenerateShooterBehaviourCollection(behaviourController, behaviourTypeOrder);
        behaviourController.currentBehaviourCollection = behaviourCollection;
        behaviourController.behaviourCollections.Add(behaviourCollection);
    }
}
