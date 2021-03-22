using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PedestrianFactory : MonoBehaviour
{
    public float lowRangeRespawnTime = 5f;
    public float highRangeRespawnTime = 10f;
    public int maximumPedestrianCount = 20;
    public List<PedestrianProbability> pedestrianProbabilities;

    private PedestrianPoint[] pedestrianPoints;
    private List<PedestrianPoint> evacuAgentSpawnPedestrianPoints;
    private bool isUsingEvacuationBehaviour = false;
    private AbstractEvacuAgentPedestrianFactory pedestrianBehaviourFactory;
    private int numberOfShooterAgentsSpawned = 0;
    private int numberOfWorkerAgentsSpawned = 0;
    private WorkerPedestrianFactory workerPedestrianFactory;
    private ShooterPedestrianFactory shooterPedestrianFactory;

    void Start()
    {
        if (pedestrianProbabilities.Count == 0)
        {
            throw new System.Exception("There are no pedestrians to spawn.");
        }
        pedestrianPoints = FindObjectsOfType<PedestrianPoint>();
        evacuAgentSpawnPedestrianPoints = new List<PedestrianPoint>();
        IsUsingEvacuationBehaviour();
        GetSpawnPedestrianPoints();
        pedestrianBehaviourFactory = gameObject.GetComponent<AbstractEvacuAgentPedestrianFactory>();
        workerPedestrianFactory = gameObject.GetComponent<WorkerPedestrianFactory>();
        shooterPedestrianFactory = gameObject.GetComponent<ShooterPedestrianFactory>();
        StartCoroutine(GeneratePedestrians());
    }

    IEnumerator GeneratePedestrians()
    {
        while (true)
        {
            yield return new WaitForSeconds(RandomNumberGenerator.GetInstance().Range(lowRangeRespawnTime, highRangeRespawnTime));
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
        if (SceneManager.GetActiveScene().name.Equals(EvacuAgentSceneParamaters.SCENE_NAME))
        {
            isUsingEvacuationBehaviour = true;
            maximumPedestrianCount = GetNumberOfEvacuAgentPedestrians();
        }
    }

    private int GetNumberOfEvacuAgentPedestrians()
    {
        return EvacuAgentSceneParamaters.NUMBER_OF_WORKER_AGENTS + EvacuAgentSceneParamaters.NUMBER_OF_SHOOTER_AGENTS;
    }

    private PedestrianPoint GetPedestrianPoint()
    {
        if (isUsingEvacuationBehaviour)

            return evacuAgentSpawnPedestrianPoints[RandomNumberGenerator.GetInstance().Range(0, evacuAgentSpawnPedestrianPoints.Count)];

        return pedestrianPoints[RandomNumberGenerator.GetInstance().Range(0, pedestrianPoints.Length)];
    }

    private void GetSpawnPedestrianPoints()
    {
        foreach(PedestrianPoint pedestrianPoint in pedestrianPoints)
        {
            if (pedestrianPoint.PedestrianPointType.Equals(PedestrianPointType.Spawn))
                evacuAgentSpawnPedestrianPoints.Add(pedestrianPoint);
        }
    }

    private void AddEvacuAgentBehaviour(Pedestrian pedestrian)
    {
        AbstractEvacuAgentPedestrianFactory evacuAgentPedestrianFactory = null;

        if (numberOfShooterAgentsSpawned < EvacuAgentSceneParamaters.NUMBER_OF_SHOOTER_AGENTS)
        {
            numberOfShooterAgentsSpawned++;
            evacuAgentPedestrianFactory = shooterPedestrianFactory;
        }
        else if (numberOfWorkerAgentsSpawned < EvacuAgentSceneParamaters.NUMBER_OF_WORKER_AGENTS)
        {
            numberOfWorkerAgentsSpawned++;
            evacuAgentPedestrianFactory = workerPedestrianFactory;
        }

        evacuAgentPedestrianFactory.CreateEvacuaAgentPedestrian(pedestrian);
    }

    [System.Serializable]
    public class PedestrianProbability
    {
        public Pedestrian pedestrian;
        public float probability;
    }
}
