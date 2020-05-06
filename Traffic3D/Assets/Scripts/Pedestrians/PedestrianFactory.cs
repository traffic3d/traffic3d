using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PedestrianFactory : MonoBehaviour
{
    public float lowRangeRespawnTime = 5f;
    public float highRangeRespawnTime = 10f;
    public int maximumPedestrianCount = 20;
    public List<PedestrianProbability> pedestrianProbabilities;

    private PedestrianPoint[] pedestrianPoints;

    void Start()
    {
        if(pedestrianProbabilities.Count == 0)
        {
            throw new System.Exception("There are no pedestrians to spawn.");
        }
        GameObject[] pathways = GameObject.FindGameObjectsWithTag("pathway");
        foreach(GameObject pathway in pathways)
        {
            pathway.GetComponents<MeshRenderer>();
            NavMeshModifier navMeshModifier = pathway.AddComponent<NavMeshModifier>();
            navMeshModifier.ignoreFromBuild = false;
            navMeshModifier.overrideArea = true;
            navMeshModifier.area = 0;
        }
        GameObject[] roadways = GameObject.FindGameObjectsWithTag("roadway");
        foreach (GameObject roadway in roadways)
        {
            roadway.GetComponents<MeshRenderer>();
            NavMeshModifier navMeshModifier = roadway.AddComponent<NavMeshModifier>();
            navMeshModifier.ignoreFromBuild = false;
            navMeshModifier.overrideArea = true;
            navMeshModifier.area = 3;
        }
        NavMeshSurface navMeshSurface = GameObject.FindObjectOfType<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();
        pedestrianPoints = FindObjectsOfType<PedestrianPoint>();
        StartCoroutine(GeneratePedestrians());
    }

    IEnumerator GeneratePedestrians()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(lowRangeRespawnTime, highRangeRespawnTime));
            print(System.DateTimeOffset.Now.ToUnixTimeMilliseconds() + " Spawn");
            if (FindObjectsOfType<Pedestrian>().Length < maximumPedestrianCount)
            {
                SpawnPedestrian();
            }
        }
    }

    public void SpawnPedestrian()
    {
        PedestrianPoint pedestrianPoint = pedestrianPoints[Random.Range(0, pedestrianPoints.Length)];
        Instantiate(GetRandomPedestrian(), pedestrianPoint.GetPointLocation(), pedestrianPoint.transform.rotation);
    }

    public Pedestrian GetRandomPedestrian()
    {
        float finalProbability = Random.value;
        float cumulativeProbability = 0.0F;
        foreach (PedestrianProbability pedestrianProbability in pedestrianProbabilities)
        {
            cumulativeProbability += pedestrianProbability.probability;
            if (finalProbability <= cumulativeProbability)
            {
                return pedestrianProbability.pedestrian;
            }
        }
        return pedestrianProbabilities.Aggregate((highest, next) => highest.probability > next.probability ? highest : next).pedestrian;
    }

    [System.Serializable]
    public class PedestrianProbability
    {
        public Pedestrian pedestrian;
        public float probability;
    }

}
