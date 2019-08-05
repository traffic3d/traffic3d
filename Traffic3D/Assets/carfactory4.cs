﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carfactory4 : MonoBehaviour
{


    public Rigidbody car1;
    public Rigidbody car3;
    public Vector3 spawnSpot1;
    public Vector3 spawnSpot3;
    int carGenerator = 0;


    // Use this for initialization
    void Start()
    {

        Random.seed = 78;
        carGenerator = 0;
        StartCoroutine(generateCars());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator generateCars()
    {

        while (true)
        {

            yield return new WaitForSeconds(Random.Range(25, 32));

            if (carCounterFactory4.carCount < Random.Range(1, 3))
            {
                if (carGenerator == 0)
                {
                    Instantiate(car1, spawnSpot1, Quaternion.Euler(Vector3.up * 180));
                    carGenerator = 1;
                    carCounterFactory4.incrementCarCount();

                }
                else
                {
                    Instantiate(car3, spawnSpot3, Quaternion.Euler(Vector3.up * 180));
                    carGenerator = 0;
                    carCounterFactory4.incrementCarCount();

                }

            }


        }
    }

}
