using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CARfac25 : MonoBehaviour {

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
	}

	// Update is called once per frame
	void Update () {

	}

	IEnumerator generateCars1() {
		while (true)
		{
			yield return new WaitForSeconds (Random.Range (1, 2));
			if (newCarCount.getCarCount () < Random.Range (2, 5)) {
				if (carGenerator == 0) {
					Instantiate (car1, spawnSpot1, Quaternion.Euler (Vector3.up * 270));
					newCarCount.incrementCarCount();
					carGenerator = 1;
				}
				else {
					Instantiate (car3, spawnSpot3, Quaternion.Euler (Vector3.up * 270));
					newCarCount.incrementCarCount();
					carGenerator = 0;
				}

			}

		}

	}

}

