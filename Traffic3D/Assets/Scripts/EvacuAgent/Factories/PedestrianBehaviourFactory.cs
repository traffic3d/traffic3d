using System;
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

    private int numberOfShooterAgentsSpawned = 0;

    public void AddEvacuAgentBehaviour(Pedestrian pedestrian)
    {
        GameObject behaviourControllerInstance = Instantiate(behaviourControllerPrefab, pedestrian.transform);
        BehaviourController behaviourController = behaviourControllerInstance.GetComponent<BehaviourController>();

        if (pedestrian.GetComponentInChildren<FieldOfView>() == null)
        {
            GameObject fieldOfView = Instantiate(behaviourController.fieldOfView, behaviourControllerInstance.transform.position, Quaternion.identity);
            fieldOfView.transform.SetParent(behaviourControllerInstance.transform);
            fieldOfView.GetComponent<MeshRenderer>().enabled = EvacuAgentSceneParamaters.IS_FOV_VISUAL_ENABLED;
        }

        if(numberOfShooterAgentsSpawned < EvacuAgentSceneParamaters.NUMBER_OF_SHOOTER_AGENTS)
        {
            numberOfShooterAgentsSpawned++;
            pedestrian.isShooterAgent = true;
            AddTag(pedestrian, EvacuAgentSceneParamaters.SHOOTER_TAG);
            AddPedestrianHighlighter(pedestrian, shooterHighlightPrefab, EvacuAgentSceneParamaters.IS_SHOOTER_HIGHTLIGHT_VISUAL_ENABLED);
            AddBehaviourCollection(behaviourController, typeof(ShooterBehaviourTypes));
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

    private void AddBehaviourCollection(BehaviourController behaviourController, Type behaviourType)
    {
        BehaviourCollection behaviourCollection = shooterBehaviourCollectionFactory.GenerateShooterBehaviourCollection(behaviourController, behaviourType);
        behaviourController.currentBehaviourColection = behaviourCollection;
        behaviourController.behaviourCollections.Add(behaviourCollection);
    }
}
