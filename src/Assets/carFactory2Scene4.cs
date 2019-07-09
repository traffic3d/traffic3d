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
	}

	// Update is called once per frame
	void Update()
	{

	}

	IEnumerator generateCars1()
	{
		print ("first corou factory2");
		//for(int i = 0; i <2; i++)
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(2, 7));
			if (newCarCount.getCarCount() < Random.Range(1,4))
			{
				if (carGenerator == 0)
				{
					Instantiate(car1, spawnSpot1, Quaternion.Euler(Vector3.up * 270));
					CARcOUNTER2sCEne3.incrementCarCount ();
					carGenerator = 1;
				}
				else
				{
					Instantiate(car3, spawnSpot3, Quaternion.Euler(Vector3.up * 270));
					CARcOUNTER2sCEne3.incrementCarCount ();
					carGenerator = 0;
				}
			}


		}
	}

}
