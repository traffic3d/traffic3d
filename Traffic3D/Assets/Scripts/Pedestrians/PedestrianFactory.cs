﻿using System.Collections;
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

    [SerializeField]
    private GameObject fieldOfViewPrefab;

    [SerializeField]
    private GameObject testCanvasPrefab;

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
        }
    }

    private void AddEvacuAgentBehaviour(Pedestrian pedestrian)
    {
        if (pedestrian.GetComponentInChildren<FieldOfView>() == null)
        {
            GameObject fieldOfView = Instantiate(fieldOfViewPrefab, pedestrian.transform.position, Quaternion.identity);
            fieldOfView.transform.SetParent(pedestrian.transform);
            fieldOfView.GetComponent<MeshRenderer>().enabled = EvacuAgentSceneParamaters.IS_FOV_VISUAL_ENABLED;
        }

        pedestrian.gameObject.AddComponent<PedestrianPathCreator>();
    }

    [System.Serializable]
    public class PedestrianProbability
    {
        public Pedestrian pedestrian;
        public float probability;
    }

}
