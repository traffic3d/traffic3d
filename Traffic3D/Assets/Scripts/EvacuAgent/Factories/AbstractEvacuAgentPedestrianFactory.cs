using System;
using UnityEngine;

public abstract class AbstractEvacuAgentPedestrianFactory : MonoBehaviour
{
    [SerializeField]
    private GameObject behaviourCollectionPrefab;

    [SerializeField]
    protected GameObject pedestrianTypePrefab;

    public abstract EvacuAgentPedestrianBase CreateEvacuaAgentPedestrian(Pedestrian pedestrian);

    protected void AddPedestrianHighlighter(Pedestrian pedestrian, GameObject prefab, bool isHighlightEnabled)
    {
        GameObject highlight = Instantiate(prefab, pedestrian.transform.position, Quaternion.identity);
        highlight.transform.SetParent(pedestrian.transform);
        highlight.GetComponent<MeshRenderer>().enabled = isHighlightEnabled;
    }

    protected void AddBehaviourCollection(BehaviourController behaviourController, BehaviourTypeOrder behaviourTypeOrder)
    {
        BehaviourCollection behaviourCollection = GenerateBehaviourCollection(behaviourController, behaviourTypeOrder);
        behaviourController.currentBehaviourCollection = behaviourCollection;
        behaviourController.behaviourCollections.Add(behaviourCollection);
    }

    public BehaviourCollection GenerateBehaviourCollection(BehaviourController behaviourController, BehaviourTypeOrder behaviourTypeOrder)
    {
        GameObject behaviourCollectionInstance = Instantiate(behaviourCollectionPrefab, behaviourController.transform);
        BehaviourCollection behaviourCollection = behaviourCollectionInstance.GetComponent<BehaviourCollection>();
        behaviourCollectionInstance.transform.SetParent(behaviourController.transform);

        foreach (BehaviourType behaviourType in behaviourTypeOrder.GetBehaviourTypes())
        {
            string behaviourTypeName = behaviourType.GetBehaviourStrategyName();
            float behaviourTypeChance = behaviourType.GetBehaviourStrategyChance();

            if (BehaviourChanceCheck(behaviourTypeChance))
            {
                BehaviourStrategy behaviourStrategy = (BehaviourStrategy)behaviourCollection.gameObject.AddComponent(Type.GetType(behaviourTypeName));
                behaviourCollection.behaviours.Add(behaviourStrategy);
            }
        }

        return behaviourCollection;
    }

    private bool BehaviourChanceCheck(float behaviourChance)
    {
        if (UnityEngine.Random.value < behaviourChance)
            return true;

        return false;
    }
}
