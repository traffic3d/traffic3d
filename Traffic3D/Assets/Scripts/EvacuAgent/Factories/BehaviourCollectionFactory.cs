using System;
using UnityEngine;

public class BehaviourCollectionFactory : MonoBehaviour
{
    [SerializeField]
    private GameObject behaviourCollectionPrefab;

    public BehaviourCollection GenerateShooterBehaviourCollection(BehaviourController behaviourController, Type behaviourTypeEnum)
    {
        GameObject behaviourCollectionInstance = Instantiate(behaviourCollectionPrefab, behaviourController.transform);
        BehaviourCollection behaviourCollection = behaviourCollectionInstance.GetComponent<BehaviourCollection>();
        behaviourCollectionInstance.transform.SetParent(behaviourController.transform);

        foreach (Enum behaviourType in Enum.GetValues(behaviourTypeEnum))
        {
            BehaviourTypeAttribute componentAttribute = BehaviourCollectionFactoryHelper.GetAttribute<BehaviourTypeAttribute>(behaviourType);
            BehaviourStrategy behaviourStrategy = (BehaviourStrategy)behaviourCollection.gameObject.AddComponent(Type.GetType(componentAttribute.BehaviourStrategyName));
            behaviourCollection.behaviours.Add(behaviourStrategy);
        }

        return behaviourCollection;
    }
}
