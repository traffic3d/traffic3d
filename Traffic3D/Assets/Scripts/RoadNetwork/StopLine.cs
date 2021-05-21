using UnityEngine;

public class StopLine : MonoBehaviour
{
    public Type type = Type.MERGE;

    public enum Type
    {
        /// <summary>
        /// Used for when a road is merging into another road.
        /// The waiting vehicle will run all checks for incoming vehicles.
        /// </summary>
        MERGE,
        /// <summary>
        /// Used for junctions without traffic lights.
        /// The waiting vehicle will run all checks for incoming vehicles from the right if driving on the left and the left if driving on the right.
        /// </summary>
        JUNCTION
    }
}
