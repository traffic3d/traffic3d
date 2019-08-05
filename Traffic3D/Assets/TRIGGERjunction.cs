using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRIGGERjunction : MonoBehaviour
{


    void Start()
    {

    }




    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        other.attachedRigidbody.tag = "drive";
    }


}
