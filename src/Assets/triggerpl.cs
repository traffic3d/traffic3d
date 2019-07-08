using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerpl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

void OnTriggerEnter(Collider other)
{
other.attachedRigidbody.tag = "rid";
//print("rid tagged");
}
}
