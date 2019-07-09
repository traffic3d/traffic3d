using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carfactory3 : MonoBehaviour {

	public Rigidbody car1;
	public Rigidbody car3;
	public Vector3 spawnSpot1;
	public Vector3 spawnSpot3;
	int carGenerator = 0;



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
			if (newCarCount.getCarCount() < Random.Range(1,7))
			{
				if (carGenerator == 0)
				{
					Instantiate (car1, spawnSpot1, Quaternion.identity);
					carGenerator = 1;
				}
				else
				{
					Instantiate(car3, spawnSpot3, Quaternion.identity);
					carGenerator = 0;
				}
			
				carCounterFACTORY3.incrementCarCount();

			}


		}
	}

}
