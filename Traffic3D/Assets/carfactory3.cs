using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carfactory3 : MonoBehaviour {

	public Rigidbody car1;
	//public Rigidbody car2;
	public Rigidbody car3;
	public Vector3 spawnSpot1;
	//public Vector3 spawnSpot2;
	public Vector3 spawnSpot3;
	public int carGenerator; 



	void Start () {
//Random.seed = 56; 
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
			yield return new WaitForSeconds(Random.Range(23,33));   //33,43
			//if (newCarCount.getCarCount() < Random.Range(4,6))       //newCarCount.maxCarNumbers())
			//for(carCounterFACTORY3.carCount=0; carCounterFACTORY3.carCount < 4; carCounterFACTORY3.carCount++)
				
			//if(carCounterFACTORY3.carCount <   3)  //Random.Range(4,6))
			if (carCounterFACTORY3.carCount < Random.Range (1, 3)) 
			{
				if (carGenerator == 0)
				{
					Instantiate (car1, spawnSpot1, Quaternion.identity); 
					//Instantiate (car3, spawnSpot3, Quaternion.identity);
					carGenerator = 1;
					carCounterFACTORY3.incrementCarCount ();
					//if (carGenerator == 0)
					//{
					//	Instantiate (car1, spawnSpot1, Quaternion.identity);        //Euler(Vector3.up * 90));
					//	carGenerator = 1;
					//	carCounterFACTORY3.incrementCarCount ();

				}
			
				//else   //(carGenerator == 1) 
				
				//if(carGenerator == 1)
				else
				{
					Instantiate (car3, spawnSpot3, Quaternion.identity);
					//Instantiate(car3, spawnSpot3, Quaternion.identity);
                     carGenerator = 0;
					carCounterFACTORY3.incrementCarCount ();
                 } 

			/*	else
				{
					Instantiate(car3, spawnSpot3, Quaternion.identity);        //Euler(Vector3.up * 270));
					carGenerator = 0;
					carCounterFACTORY3.incrementCarCount ();

				}   */
			
				//carCounterFACTORY3.incrementCarCount ();
				//carCounterFactory4.incrementCarCount ();

				//newCarCount.incrementCarCount();
			}


		}
	}
	
}