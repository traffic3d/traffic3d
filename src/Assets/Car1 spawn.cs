using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car1spawn : MonoBehaviour
{

    public GameObject Car1;
    public Vector3 spawnLocation = new Vector3(-17.38f, 11.28f, -13.73f);
    // Use this for initialization
    void Start()
    {
        GameObject Car1spawn = (GameObject)Instantiate(Car1, spawnLocation, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
