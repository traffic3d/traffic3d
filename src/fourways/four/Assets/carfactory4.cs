using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carfactory4 : MonoBehaviour {


	public Rigidbody car1;
	//public Rigidbody car2;
	public Rigidbody car3;
	public Vector3 spawnSpot1;
	//public Vector3 spawnSpot2;
	public Vector3 spawnSpot3;
	int carGenerator = 0;


	// Use this for initialization
	void Start () {
		carGenerator = 0;
		StartCoroutine(generateCars());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator generateCars()
	{

		while (true)
		{
			yield return new WaitForSeconds(Random.Range(2, 7));
			if (newCarCount.getCarCount() < Random.Range(1,7))       //newCarCount.maxCarNumbers())
			{
				if (carGenerator == 0)
				{
					Instantiate (car1, spawnSpot1, Quaternion.Euler(Vector3.up * 180));        //(Euler.Vector3.up * 360));    //
					carGenerator = 1;
				}
				/* else if (carGenerator == 1) {
                     Instantiate(car2, spawnSpot2, Quaternion.Euler(Vector3.up * 90));
                     carGenerator = 2;
                 } */
				else
				{
					Instantiate(car3, spawnSpot3, Quaternion.Euler(Vector3.up *180));
					carGenerator = 0;
				}

				carCounterFactory4.incrementCarCount ();

				//newCarCount.incrementCarCount();
			}


		}
	}

}
