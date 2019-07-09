using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CARfactory3 : MonoBehaviour
{

    public Rigidbody car1;
    public Rigidbody car2;
    public Rigidbody car3;
    public Vector3 spawnSpot1;
    public Vector3 spawnSpot2;
    public Vector3 spawnSpot3;
    int carGenerator = 0;

    // Use this for initialization
    void Start()
    {
        carGenerator = 0;
        StartCoroutine(carloop());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator carloop()
    {
        while (true)
        {

            yield return StartCoroutine(generateCars1());
            yield return StartCoroutine(generateCars2());

        }
    }

    IEnumerator generateCars1()
    {
        {
            yield return new WaitForSeconds(Random.Range(2, 4));
            if (CarCounter.getCarCount() < Random.Range(2, 3))
            {
                if (carGenerator == 0)
                {
                    Instantiate(car1, spawnSpot1, Quaternion.Euler(Vector3.up * 90));
                    CarCounter.incrementCarCount();
                    carGenerator = 1;
                }
                else
                {
                    Instantiate(car3, spawnSpot3, Quaternion.Euler(Vector3.up * 90));
                    CarCounter.incrementCarCount();
                    carGenerator = 0;
                }

            }
        }

    }


    IEnumerator generateCars2()
    {
        {
            yield return new WaitForSeconds(Random.Range(2, 4));
            if (CarCounter.getCarCount() < Random.Range(1, 5))
            {
                if (carGenerator == 0)
                {
                    Instantiate(car1, spawnSpot1, Quaternion.Euler(Vector3.up * 90));
                    CarCounter.incrementCarCount();
                    carGenerator = 1;
                }
                else
                {
                    Instantiate(car3, spawnSpot3, Quaternion.Euler(Vector3.up * 90));
                    CarCounter.incrementCarCount();
                    carGenerator = 0;
                }

            }

        }

    }

}
