using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carFactory2Scene4 : MonoBehaviour {

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
		StartCoroutine(generateCars1());
		//	StartCoroutine(generateCars2());
	}

	// Update is called once per frame
	void Update()
	{

	}

	/*void createCar() {
        Random.Range(0.0f, 1.0f);


        Instantiate(car1, spawnSpot1, Quaternion.identity);
        Instantiate(car2, spawnSpot2, Quaternion.identity);
        Instantiate(car3, spawnSpot3, Quaternion.identity);

    }   */

	IEnumerator generateCars1()
	{
		print ("first corou factory2");
		//for(int i = 0; i <2; i++)
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(2, 7));
			if (newCarCount.getCarCount() < Random.Range(1,4))       //newCarCount.maxCarNumbers())
			{
				if (carGenerator == 0)
				{
					Instantiate(car1, spawnSpot1, Quaternion.Euler(Vector3.up * 270));
					CARcOUNTER2sCEne3.incrementCarCount ();
					carGenerator = 1;
				}
				/* else if (carGenerator == 1) {
                     Instantiate(car2, spawnSpot2, Quaternion.Euler(Vector3.up * 90));
                     carGenerator = 2;
                 } */
				else
				{
					Instantiate(car3, spawnSpot3, Quaternion.Euler(Vector3.up * 270));
					CARcOUNTER2sCEne3.incrementCarCount ();
					carGenerator = 0;
				}
				//newCarCount.incrementCarCount();
			//	CARcOUNTER2sCEne3.incrementCarCount ();
				//carCounterScene3.incrementCarCount ();
			}


		}
	}
	/*	IEnumerator generateCars2()
	{
		print ("first corou factory2");
		for(int i = 0; i <2; i++)

		{
			yield return new WaitForSeconds(Random.Range(2, 7));
			if (newCarCount.getCarCount() < Random.Range(1,2))       //newCarCount.maxCarNumbers())
			{
				if (carGenerator == 0)
				{
					Instantiate(car1, spawnSpot1, Quaternion.Euler(Vector3.up * 270));
					carGenerator = 1;
				}
				 else if (carGenerator == 1) {
                     Instantiate(car2, spawnSpot2, Quaternion.Euler(Vector3.up * 90));
                     carGenerator = 2;
                 } 
				else
				{
					Instantiate(car3, spawnSpot3, Quaternion.Euler(Vector3.up * 270));
					carGenerator = 0;
				}
				newCarCount.incrementCarCount();
			}


		}
	}  */

}
