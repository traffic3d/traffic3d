using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class printtest : MonoBehaviour
{


    void Start()
    {


        StartCoroutine(next());
    }


    public IEnumerator next()
    {
        while (true)
        {
            yield return new WaitForSeconds(30);
            SceneManager.LoadSceneAsync("Scene3");

        }


    }

    void Update()
    {

    }
}
