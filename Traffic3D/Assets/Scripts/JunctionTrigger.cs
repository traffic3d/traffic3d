using UnityEngine;

public class JunctionTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        other.attachedRigidbody.tag = "drive";
    }
}