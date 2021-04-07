using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FriendGroupBoidBehaviour : BoidBehaviourStrategyBase
{
    public override float CohesionWeight { get => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_COHESION_WEIGHT; }
    public override float AlignmentWeight { get => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_ALIGNMENT_WEIGHT; }
    public override float SeparationWeight { get => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_SEPARATION_WEIGHT; }
    public override float SeekingWeight { get => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_SEEKING_WEIGHT; }
    public override float AvoidanceWeight { get => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_AVOIDANCE_WEIGHT; }
    protected override float Speed { get => 15f; }

    private int walkableAreaMask;
    private float radiusToSampleNavMesh;

    void Start()
    {
        base.Start();
        Neighbours = new List<BoidBehaviourStrategyBase>();
        BoidManager = GetComponentInParent<BoidManager>();
        walkableAreaMask = EvacuAgentPedestrianBase.pedestrian.GetWalkableAreaMaske();
        radiusToSampleNavMesh = 5f;
        StartCoroutine(BoidCoolDown());
    }

    public override void PerformBehaviour()
    {
        UpdateNeighbours();
        Target = GroupCollection.GroupDestination;
        Vector3 newVelocity = BoidManager.CalculateNewVelocity();
        newVelocity = newVelocity.normalized;
        newVelocity *= Speed;
        newVelocity = LimitVelocity(newVelocity);
        newVelocity = LimitRotation(newVelocity);
        Vector3 newPosition = transform.root.position + newVelocity;

        if (IsDestinationValid(newPosition))
        {
            NavMeshAgent.SetDestination(newPosition);
            Velocity = newVelocity;
        }
        else // Instead of this I should make agents seen obstacles and add an obstacle avoidance component similar to separation
        {
            /*NavMesh.FindClosestEdge(newPosition, out NavMeshHit navMeshHit, NavMesh.AllAreas);
            NavMeshAgent.SetDestination(navMeshHit.position);*/

            Vector3 point;
            if (RandomPoint(newPosition, radiusToSampleNavMesh, out point))
            {
                NavMeshAgent.SetDestination(point);
            }
        }
    }

    public override bool ShouldTriggerBehaviour()
    {
        return shouldUpdateBoid;
    }

    public void SetPositionAndVelocity(Vector3 position, Vector3 velocity)
    {
        Position = position;
        Velocity = velocity;
    }

    IEnumerator BoidCoolDown()
    {
        while (true)
        {
            shouldUpdateBoid = true;
            yield return new WaitForSeconds(0.1f);
            shouldUpdateBoid = false;
        }
    }

    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, walkableAreaMask))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
}
