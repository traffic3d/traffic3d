﻿using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Pedestrian : MonoBehaviour
{
    public float walkingInRoadPercentage = 0.01f;
    public float partialPathFallbackLocationDistance = 5f;
    public float maxSpeed = 5f;
    public float minSpeed = 1f;
    public float probabilityOfRunning = 0.1f;
    public bool isUsingEvacuationBehaviour;
    private FieldOfView fieldOfView;

    [SerializeField]
    private bool isShooterAgent;

    [SerializeField]
    private PedestrianPoint[] currentPath;

    // If changed, the animation controller needs the running value changed too.
    private float runSpeed = 2.5f;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Vector3 location;
    private int walkingInRoadAreaCost = 5;
    private float normalSpeed;
    private float normalStoppingDistance;
    private Vector3[] generalPathCorners = null;
    private bool allowCrossing = true;
    private int roadAreaMask;
    private int walkableAreaMask;
    private int pedestrianCrossingAreaMask;
    private int shooterSeekingFrequency = 4;
    private float navMeshSampleDistance = 30f;
    private PedestrianPathCreator pedestrianPathCreator;
    private int currentPathIndex = 0;
    private int sizeOfPath = 4;

    void Start()
    {
        // Shifts are needed when combining the masks to make multiple areas accessible to the Pedestrian.
        roadAreaMask = 1 << NavMesh.GetAreaFromName(PedestrianManager.ROAD_AREA);
        walkableAreaMask = 1 << NavMesh.GetAreaFromName(PedestrianManager.WALKABLE_AREA);
        pedestrianCrossingAreaMask = 1 << NavMesh.GetAreaFromName(PedestrianManager.PEDESTRIAN_CROSSING_AREA);
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (UnityEngine.Random.value > probabilityOfRunning)
        {
            navMeshAgent.speed = UnityEngine.Random.Range(minSpeed, runSpeed);
        }
        else
        {
            navMeshAgent.speed = UnityEngine.Random.Range(runSpeed, maxSpeed);
        }
        normalSpeed = navMeshAgent.speed;
        normalStoppingDistance = navMeshAgent.stoppingDistance;
        navMeshAgent.avoidancePriority = UnityEngine.Random.Range(0, 99);
        // Chance of walking in road
        if (UnityEngine.Random.value > walkingInRoadPercentage)
        {
            // Walkable
            navMeshAgent.areaMask = walkableAreaMask | pedestrianCrossingAreaMask;
        }
        else
        {
            // Walkable and Road
            navMeshAgent.areaMask = walkableAreaMask | roadAreaMask | pedestrianCrossingAreaMask;
            navMeshAgent.SetAreaCost(roadAreaMask, walkingInRoadAreaCost);
        }

        if (isUsingEvacuationBehaviour)
        {
            currentPath = new PedestrianPoint[sizeOfPath];
            fieldOfView = GetComponentInChildren<FieldOfView>();
            pedestrianPathCreator = GetComponent<PedestrianPathCreator>();
        }

        GoToRandomPossibleLocation();
    }

    void Update()
    {
        animator.SetFloat("speed", navMeshAgent.velocity.magnitude);

        if (!isUsingEvacuationBehaviour)
        {
            DestroyCheck();
            CrossingCheck();
            SpeedUpdate();
            LocationUpdate();
        }
        else
        {
            DestroyCheck();

            if (isShooterAgent == true)
            {
                ShooterSeekingBehaviour();
            }
            else
            {
                LocationUpdate();
            }
        }
    }

    public void DestroyCheck()
    {
        if (Vector3.Distance(transform.position, location) < 1f)
        {
            if (isUsingEvacuationBehaviour)
            {
                GoToRandomPossibleLocation();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void CrossingCheck()
    {
        if (allowCrossing)
        {
            if (navMeshAgent.areaMask == walkableAreaMask)
            {
                navMeshAgent.areaMask = walkableAreaMask | pedestrianCrossingAreaMask;
            }
        }
        else
        {
            if (navMeshAgent.areaMask == (walkableAreaMask | pedestrianCrossingAreaMask))
            {
                NavMeshHit navMeshHit;
                if (NavMesh.SamplePosition(transform.position, out navMeshHit, 1f, walkableAreaMask))
                {
                    navMeshAgent.areaMask = walkableAreaMask;
                }
            }
        }
    }

    public void SpeedUpdate()
    {
        if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            navMeshAgent.speed = 0;
        }
        else if (navMeshAgent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            navMeshAgent.speed = normalSpeed;
            navMeshAgent.stoppingDistance = normalStoppingDistance;
            Vector3 nearestLocation = FindNearestGeneralPathPoint();
            if (Vector3.Distance(transform.position, nearestLocation) < partialPathFallbackLocationDistance)
            {
                navMeshAgent.SetDestination(nearestLocation);
            }
            else
            {
                navMeshAgent.SetDestination(transform.position);
            }
        }
        else
        {
            navMeshAgent.speed = normalSpeed;
        }
    }

    public void LocationUpdate()
    {
        if (navMeshAgent.destination != location)
        {
            if (CanContinue())
            {
                GoToFinalDestination();
            }
        }
    }

    public void SetAllowCrossing(bool allowCrossing)
    {
        this.allowCrossing = allowCrossing;
    }

    public bool GetAllowCrossing()
    {
        return this.allowCrossing;
    }

    public void GoToRandomPossibleLocation()
    {
        location = FindRandomPossibleLocation();
        if (location.Equals(Vector3.negativeInfinity))
        {
            Destroy(gameObject);
            return;
        }
        GoToFinalDestination();
    }

    public void GoToFinalDestination()
    {
        navMeshAgent.SetDestination(location);
        navMeshAgent.stoppingDistance = 0;
    }

    public Vector3 FindRandomPossibleLocation()
    {
        foreach (PedestrianPoint pedestrianPoint in FindObjectsOfType<PedestrianPoint>().OrderBy(a => Guid.NewGuid()).ToList())
        {
            Vector3 location = pedestrianPoint.GetPointLocation();
            if (Vector3.Distance(location, transform.position) < 1)
            {
                continue;
            }
            NavMeshPath path = new NavMeshPath();
            if (navMeshAgent.CalculatePath(location, path))
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    generalPathCorners = path.corners;
                    return location;
                }
            }
        }
        return Vector3.negativeInfinity;
    }

    private bool CanContinue()
    {
        NavMeshPath path = new NavMeshPath();

        if (navMeshAgent.isOnNavMesh && navMeshAgent.CalculatePath(location, path))
        {
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                return true;
            }
        }
        return false;
    }

    private Vector3 FindNearestGeneralPathPoint()
    {
        return generalPathCorners.Aggregate((nearestPoint, next) => Vector3.Distance(transform.position, next) < Vector3.Distance(transform.position, nearestPoint) ? next : nearestPoint);
    }

    public void ShooterSeekingBehaviour()
    {
        // Follow closest target in field of view
        if (fieldOfView.visiblePedestrians.Count >= 1)
        {
            currentPathIndex = sizeOfPath;
            StartCoroutine(FindTargetsWithDelay());
            return;
        }

        // If no visible target create a path of pedestrian points to travel to
        if (currentPath[currentPathIndex] == null || currentPathIndex + 1 >= sizeOfPath)
        {
            currentPathIndex = 0;
            currentPath = pedestrianPathCreator.CalculateRankedShooterAgentPath(100f, transform, sizeOfPath, 0.7f, 0.3f);
            navMeshAgent.SetDestination(currentPath[currentPathIndex].transform.position);
            return;
        }

        // If at current destination, assign next destination
        if (Vector3.Distance(transform.position, navMeshAgent.destination) < 1f)
        {
            currentPathIndex++;
            navMeshAgent.SetDestination(currentPath[currentPathIndex].transform.position);
            return;
        }
    }

    IEnumerator FindTargetsWithDelay()
    {
        yield return new WaitForSeconds(shooterSeekingFrequency);
        FollowClosestPedestrian();
    }

    private void FollowClosestPedestrian()
    {
        Transform cloststTransform = fieldOfView.visiblePedestrians.First().transform;
        float currentSmallestDistance = Vector3.Distance(transform.position, cloststTransform.position);

        foreach (Pedestrian pedestrian in fieldOfView.visiblePedestrians)
        {
            float distanceToAgent = Vector3.Distance(transform.position, pedestrian.transform.position);

            if (distanceToAgent > currentSmallestDistance)
            {
                cloststTransform = pedestrian.transform;
                currentSmallestDistance = distanceToAgent;
            }
        }

        navMeshAgent.ResetPath();
        navMeshAgent.SetDestination(cloststTransform.position);
    }
}
