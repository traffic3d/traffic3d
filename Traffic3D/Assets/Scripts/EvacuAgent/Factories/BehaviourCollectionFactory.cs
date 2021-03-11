using System;
using UnityEngine;

public class BehaviourCollectionFactory : MonoBehaviour
{
    [SerializeField]
    private GameObject behaviourCollectionPrefab;

    public BehaviourCollection GenerateShooterBehaviourCollection(BehaviourController behaviourController, BehaviourTypeOrder behaviourTypeOrder)
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
