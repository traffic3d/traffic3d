public class GroupFollowerPedestrian : GroupPedestrian
{
    public void ChangeSpeedToMatchLeader(float leaderSpeed)
    {
        navMeshAgent.speed = leaderSpeed;
    }
}
