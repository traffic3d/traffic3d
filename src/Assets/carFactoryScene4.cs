using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carFactoryScene4 : MonoBehaviour {

	public Rigidbody car1;
	public Rigidbody car2;
	public Rigidbody car3;
	public Vector3 spawnSpot1;
	public Vector3 spawnSpot2;
	public Vector3 spawnSpot3;
	int carGenerator = 0;

	// Use this for initialization
	void Start () {
		carGenerator = 0;
		StartCoroutine(generateCars1());
		//StartCoroutine(generateCars2());
	}

	// Update is called once per frame
	void Update () {

	}

	IEnumerator generateCars1() {
		print("entered first corou");
		while (true)
		{
			yield return new WaitForSeconds (Random.Range (2, 7));
			if (CarCounter.getCarCount () < Random.Range (2, 4)) {
				if (carGenerator == 0) {
					Instantiate (car1, spawnSpot1, Quaternion.Euler (Vector3.up * 90));
					carCounterScene3.incrementCarCount ();
					carGenerator = 1;
				}
				else {
					Instantiate (car3, spawnSpot3, Quaternion.Euler (Vector3.up * 90));
					carCounterScene3.incrementCarCount ();
					carGenerator = 0;
				}
			
			}

		}

	}


}
