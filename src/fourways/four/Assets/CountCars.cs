using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountCars : MonoBehaviour
{
    public object objectsWithTag { get; internal set; }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("car") as GameObject[];
    }


}

