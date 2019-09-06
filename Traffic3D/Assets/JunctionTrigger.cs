using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunctionTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        other.attachedRigidbody.tag = "drive";
    }
}