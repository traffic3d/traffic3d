using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Networking : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(trying());
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public IEnumerator trying()
	{
		while (true) {
		
		
			yield return new WaitForSeconds (20);
			SceneManager.LoadSceneAsync("Scene5");
		
		
		}
	}

}
