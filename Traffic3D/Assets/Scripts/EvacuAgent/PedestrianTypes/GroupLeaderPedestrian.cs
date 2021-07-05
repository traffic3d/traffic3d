public class GroupLeaderPedestrian : NonShooterPedestrian
{
    protected int numberOfFollowers;

    public int GetNumberOfFollowers()
    {
        return numberOfFollowers;
    }

    public void SetNumberOfFollowers(int numFollowers)
    {
        numberOfFollowers = numFollowers;
    }
}
