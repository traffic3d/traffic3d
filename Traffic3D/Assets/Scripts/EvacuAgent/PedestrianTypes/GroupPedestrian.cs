using UnityEngine;

public abstract class GroupPedestrian : NonShooterPedestrian
{
    [SerializeField]
    public GroupCollection GroupCollection { get; private set; }

    public void AddGroupCollection(GroupCollection groupCollection)
    {
        GroupCollection = groupCollection;
    }
}
