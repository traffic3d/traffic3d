using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFactory : MonoBehaviour {

    public Rigidbody car1;
    public Rigidbody car2;
    public Rigidbody car3;
    public Vector3 spawnSpot1;
    public Vector3 spawnSpot2;
    public Vector3 spawnSpot3;
    int carGenerator = 0;

	//public bool yes = false;
    // Use this for initialization
    void Start () {
       Random.seed = 123; 
       carGenerator = 0;
        StartCoroutine(generateCars1());
		//StartCoroutine(generateCars2());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /*void createCar() {
        Random.Range(0.0f, 1.0f);


        Instantiate(car1, spawnSpot1, Quaternion.identity);
        Instantiate(car2, spawnSpot2, Quaternion.identity);
        Instantiate(car3, spawnSpot3, Quaternion.identity);

    }   */

    IEnumerator generateCars1() {
		//print("entered first corou");
       while (true)
		//if(yes == false)
		{
			//for (int i = 0; i < 2; i++) { 
				yield return new WaitForSeconds (Random.Range (8, 10));
				if (CarCounter.carCount < Random.Range (2, 7)) //(2,13) {                   //CarCounter.maxCarNumbers)
{
					if (carGenerator == 0) {
						Instantiate (car1, spawnSpot1, Quaternion.Euler (Vector3.up * 45));
					CarCounter.incrementCarCount ();
						carGenerator = 1;
					}
               /* else if (carGenerator == 1) {
                    Instantiate(car2, spawnSpot2, Quaternion.Euler(Vector3.up * 90));
                    carGenerator = 2;
                } */
                else {
		Instantiate (car3, spawnSpot3, Quaternion.Euler (Vector3.up * 45));
					CarCounter.incrementCarCount ();	
					carGenerator = 0;
					}

				//CarCounter.incrementCarCount ();
			
			}
				
				//yes = true;
			}
           
        }

	//}

/*	IEnumerator generateCars2() {

		print("entered second corou");
		//while (true) 
		if(yes == true)
		{
			for (int i = 0; i < 2; i++) { 
				yield return new WaitForSeconds (Random.Range (2, 7));
				if (CarCounter.getCarCount () < Random.Range (1, 2)) {                   //CarCounter.maxCarNumbers)
					if (carGenerator == 0) {
						Instantiate (car1, spawnSpot1, Quaternion.Euler (Vector3.up * 90));
						carGenerator = 1;
					}
				 else if (carGenerator == 1) {
                    Instantiate(car2, spawnSpot2, Quaternion.Euler(Vector3.up * 90));
                    carGenerator = 2;
                } 
				else {
						Instantiate (car3, spawnSpot3, Quaternion.Euler (Vector3.up * 90));
						carGenerator = 0;
					}
				}
					

				CarCounter.incrementCarCount ();
				


			}
		}
	}  */
    
}
