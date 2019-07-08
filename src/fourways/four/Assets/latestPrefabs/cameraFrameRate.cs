using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFrameRate : MonoBehaviour {

	public int frameRate = 5;

	void Start () {
		Time.captureFramerate = frameRate;	
	}
	

	void Update () {
		
	}
}
