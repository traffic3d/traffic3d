using System;
using UnityEngine;

public abstract class AbstractEvacuAgentPedestrianFactory : MonoBehaviour
{
    public bool isCreatingLeaderType { get; protected set; }

    [SerializeField]
    public GameObject behaviourCollectionPrefab;

    [SerializeField]
    public GameObject leaderPedestrianTypePrefab;

    [SerializeField]
    public GameObject followerPedestrianTypePrefab;

    protected EvacuAgentPedestrianBase currentLeaderPedestrian;
    protected GroupCollection currentLeaderFollowerCollection;
    protected int numPedestriansToSpawn; // Max pedestrians to spawn
    protected int numberOfFollowersLeftToSpawn = 0; // Pedestrians left in this group to spawn

    public abstract EvacuAgentPedestrianBase CreateEvacuAgentPedestrian(Pedestrian pedestrian);

    public virtual bool HasSpawnedMaxPedestrians()
    {
        return numPedestriansToSpawn + numberOfFollowersLeftToSpawn == 0;
    }

    public BehaviourCollection GenerateBehaviourCollection(BehaviourController behaviourController, BehaviourTypeOrder behaviourTypeOrder)
    {
        GameObject behaviourCollectionInstance = Instantiate(behaviourCollectionPrefab, behaviourController.transform);
        BehaviourCollection behaviourCollection = behaviourCollectionInstance.GetComponent<BehaviourCollection>();
        behaviourCollectionInstance.transform.SetParent(behaviourController.transform);

        foreach (BehaviourType behaviourType in behaviourTypeOrder.GetBehaviourTypes())
        {
            string behaviourTypeName = behaviourType.GetBehaviourStrategyClass<BehaviourStrategy>().ToString();
            float behaviourTypeChance = behaviourType.GetBehaviourStrategyChance();

            if (BehaviourChanceCheck(behaviourTypeChance))
            {
                BehaviourStrategy behaviourStrategy = (BehaviourStrategy)behaviourCollection.gameObject.AddComponent(Type.GetType(behaviourTypeName));
                behaviourCollection.behaviours.Add(behaviourStrategy);
            }
        }

        return behaviourCollection;
    }

    public int GetNumPedestriansToSpawn()
    {
        return numPedestriansToSpawn;
    }

    public int GetNumberOfFollowers()
    {
        return numberOfFollowersLeftToSpawn;
    }

    protected EvacuAgentPedestrianBase CreatePedestrianType(Pedestrian pedestrian, bool isHighlightEnabled, GameObject pedestrianTypePrefab)
    {
        GameObject evacuAgentPedestrianObj = GameObject.Instantiate(pedestrianTypePrefab, pedestrian.transform);
        EvacuAgentPedestrianBase evacuAgentPedestrian = evacuAgentPedestrianObj.GetComponent<EvacuAgentPedestrianBase>();

        evacuAgentPedestrian.InitialisePedestrian(pedestrian);
        AddPedestrianHighlighter(pedestrian, evacuAgentPedestrian.pedestrianHighlight, isHighlightEnabled);
        evacuAgentPedestrian.behaviourController.transform.SetParent(evacuAgentPedestrian.transform);
        AddBehaviourCollection(evacuAgentPedestrian.behaviourController, evacuAgentPedestrian.behaviourTypeOrder);
        evacuAgentPedestrian.behaviourController.isUpdateOn = true;
        return evacuAgentPedestrian;
    }

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

    protected int GetNumberOfFollowersForCurrentGroup(int lowerBound, int upperBound)
    {
        return UnityEngine.Random.Range(lowerBound, upperBound);
    }

    protected EvacuAgentPedestrianBase UpdateGroupCollection()
    {
        IsCreatingLeaderType(true);
        numPedestriansToSpawn--;
        currentLeaderFollowerCollection = currentLeaderPedestrian.GetComponent<GroupCollection>();
        currentLeaderFollowerCollection.TotalGroupCount = numberOfFollowersLeftToSpawn;
        currentLeaderFollowerCollection.GroupLeaderPedestrian = currentLeaderPedestrian;
        currentLeaderFollowerCollection.AddFollowerToCollection(currentLeaderPedestrian);
        currentLeaderPedestrian.GroupCollection = currentLeaderFollowerCollection;
        return currentLeaderPedestrian;
    }

    protected void AddGroupCollectionToFollower(EvacuAgentPedestrianBase groupPedestrian)
    {
        IsCreatingLeaderType(false);
        groupPedestrian.AddGroupCollection(currentLeaderFollowerCollection);
    }

    protected EvacuAgentPedestrianBase AssignToFollowerCollection(EvacuAgentPedestrianBase follower)
    {
        numberOfFollowersLeftToSpawn--;
        currentLeaderFollowerCollection.AddFollowerToCollection(follower);
        follower.navMeshAgent.stoppingDistance = 1f;
        return follower;
    }

    private void IsCreatingLeaderType(bool isCreatingLeader)
    {
        isCreatingLeaderType = isCreatingLeader;
    }

    private bool BehaviourChanceCheck(float behaviourChance)
    {
        if (UnityEngine.Random.value < behaviourChance)
            return true;

        return false;
    }
}
