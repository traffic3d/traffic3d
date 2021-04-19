using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PedestrianFactory : MonoBehaviour
{
    public float lowRangeRespawnTime = 1f;
    public float highRangeRespawnTime = 2f;
    public int maximumPedestrianCount = 20;
    public List<PedestrianProbability> pedestrianProbabilities;

    private PedestrianPoint[] pedestrianPoints;
    private List<PedestrianPoint> evacuAgentSpawnPedestrianPoints;
    private bool isUsingEvacuationBehaviour = false;
    private List<AbstractEvacuAgentPedestrianFactory> evacuAgentPedestrianFactories;
    private int[] evacuAgentBuildIndexes = new int[] { 2, 3, 4 };

    void Start()
    {
        if (pedestrianProbabilities.Count == 0)
        {
            throw new System.Exception("There are no pedestrians to spawn.");
        }
        pedestrianPoints = FindObjectsOfType<PedestrianPoint>();
        evacuAgentSpawnPedestrianPoints = new List<PedestrianPoint>();
        evacuAgentPedestrianFactories = SetUpEvacuAgentPedestrianFactories();
        IsUsingEvacuationBehaviour();
        GetSpawnPedestrianPoints();
        StartCoroutine(GeneratePedestrians());
    }

    IEnumerator GeneratePedestrians()
    {
        while (true)
        {
            yield return new WaitForSeconds(RandomNumberGenerator.GetInstance().Range(lowRangeRespawnTime, highRangeRespawnTime));
            int debug = FindObjectsOfType<Pedestrian>().Length;
            if (FindObjectsOfType<Pedestrian>().Length < maximumPedestrianCount)
            {
                SpawnPedestrian();
            }
        }
    }

    public void SpawnPedestrian()
    {
        PedestrianPoint pedestrianPoint = pedestrianPoints[RandomNumberGenerator.GetInstance().Range(0, pedestrianPoints.Length)];
        Pedestrian pedestrian = Instantiate(GetRandomPedestrian(), pedestrianPoint.GetPointLocation(), pedestrianPoint.transform.rotation);

        if (pedestrian.isUsingEvacuationBehaviour)
        {
            AddEvacuAgentBehaviour(pedestrian);
        }
    }

    public Pedestrian GetRandomPedestrian()
    {
        float finalProbability = RandomNumberGenerator.GetInstance().NextFloat();
        float cumulativeProbability = 0.0F;
        foreach (PedestrianProbability pedestrianProbability in pedestrianProbabilities)
        {
            cumulativeProbability += pedestrianProbability.probability;
            if (finalProbability <= cumulativeProbability)
            {
                Pedestrian pedestrian = pedestrianProbability.pedestrian;
                pedestrian.isUsingEvacuationBehaviour = isUsingEvacuationBehaviour;
                return pedestrian;
            }
        }
        return pedestrianProbabilities.Aggregate((highest, next) => highest.probability > next.probability ? highest : next).pedestrian;
    }

    private void IsUsingEvacuationBehaviour()
    {
        if (evacuAgentBuildIndexes.Contains(SceneManager.GetActiveScene().buildIndex))
        {
            isUsingEvacuationBehaviour = true;
            maximumPedestrianCount = GetNumberOfEvacuAgentPedestrians();
        }
    }

    private int GetNumberOfEvacuAgentPedestrians()
    {
        int sumOfPedestriansToSpawn = 0;
        evacuAgentPedestrianFactories.ForEach(x => sumOfPedestriansToSpawn += x.GetNumPedestriansToSpawn());
        return sumOfPedestriansToSpawn;
    }

    private PedestrianPoint GetPedestrianPoint()
    {
        if (isUsingEvacuationBehaviour)
            return evacuAgentSpawnPedestrianPoints[Random.Range(0, evacuAgentSpawnPedestrianPoints.Count)];

        return pedestrianPoints[RandomNumberGenerator.GetInstance().Range(0, pedestrianPoints.Length)];
    }

    private void GetSpawnPedestrianPoints()
    {
        foreach(PedestrianPoint pedestrianPoint in pedestrianPoints)
        {
            if (pedestrianPoint.pedestrianPointType.Equals(PedestrianPointType.Spawn))
                evacuAgentSpawnPedestrianPoints.Add(pedestrianPoint);
        }
    }

    /* Due to LeaderFollowerFactories spawning a random number of followers the maximumPedestrianCount
     * must be updated to ensure that followers do not subtract from maximumPedestrianCount
     * here they are seen as extras, with the non follower types only counting towards the count
     */
    private void AddEvacuAgentBehaviour(Pedestrian pedestrian)
    {
        if(evacuAgentPedestrianFactories.Any())
        {
            AbstractEvacuAgentPedestrianFactory factory = evacuAgentPedestrianFactories[Random.Range(0, evacuAgentPedestrianFactories.Count)];
            factory.CreateEvacuAgentPedestrian(pedestrian);

            if(factory.isCreatingLeaderType)
                maximumPedestrianCount += factory.GetNumberOfFollowers();

            if (factory.HasSpawnedMaxPedestrians())
                evacuAgentPedestrianFactories.Remove(factory);
        }
    }

    private List<AbstractEvacuAgentPedestrianFactory> SetUpEvacuAgentPedestrianFactories()
    {
        return new List<AbstractEvacuAgentPedestrianFactory>()
        {
            gameObject.GetComponent<WorkerLeaderFollowerPedestrianFactory>(),
            gameObject.GetComponent<FriendGroupLeaderFollowerPedestrianFactory>()
        };
    }

    [System.Serializable]
    public class PedestrianProbability
    {
        public Pedestrian pedestrian;
        public float probability;
    }
}
