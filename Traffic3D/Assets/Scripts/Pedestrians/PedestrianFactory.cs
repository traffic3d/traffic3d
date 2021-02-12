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
    private bool isUsingEvacuationBehaviour = false;

    void Start()
    {
        if (pedestrianProbabilities.Count == 0)
        {
            throw new System.Exception("There are no pedestrians to spawn.");
        }
        pedestrianPoints = FindObjectsOfType<PedestrianPoint>();
        IsUsingEvacuationBehaviour();
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
        Instantiate(GetRandomPedestrian(), pedestrianPoint.GetPointLocation(), pedestrianPoint.transform.rotation);
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
        if (SceneManager.GetActiveScene().name.Equals(EvacuAgentSceneConstants.SCENE_NAME))
        {
            isUsingEvacuationBehaviour = true;
        }
    }

    [System.Serializable]
    public class PedestrianProbability
    {
        public Pedestrian pedestrian;
        public float probability;
    }

}
