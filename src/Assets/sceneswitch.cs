using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneswitch : MonoBehaviour {
	//public bool yes = false;

	void Start () {
		StartCoroutine (abc());
	}
	

	void Update () {
		//if (yes == true) {
		
			
		
		//}
	}

	public IEnumerator abc()
	{
		while (true) {
		
			yield return new WaitForSeconds (30);
			SceneManager.LoadSceneAsync ("Scene4");
		}

	}

}
