using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleRegeneration : MonoBehaviour {

    public Rigidbody VEHICLE;
    public Vector3 spawnSpot = new Vector3(3, 11, 5);
    public VehicleEngine ve;


                                 // Use this for initialization
    void Start () {
        ve = VEHICLE.GetComponent<VehicleEngine>();
        //Instantiate(VEHICLE, spawnSpot, Quaternion.identity);
        


    }
	
	// Update is called once per frame
	void Update () {
        if (ve.currentNode == ve.nodes.Count )
        {
            VEHICLE = (Rigidbody)Instantiate(VEHICLE, spawnSpot, Quaternion.identity);
            ve = VEHICLE.GetComponent<VehicleEngine>();
            try
            {
                var a = ve.path == null;
                print("vehicle has a path");
            }
            catch (System.Exception e) { 
            
                print("vehicle has No path");
                print(e.ToString());
            }
            ve.setUpPath(VEHICLE.GetComponent<Path>().GetComponentsInChildren<Transform>());

            //ve = VEHICLE.GetComponent<VehicleEngine>();
            
        }
    }
 }
