using DigitalRuby.RainMaker;
using UnityEngine;

public class EnvironmentSettings : MonoBehaviour
{
    public bool rain;
    public bool snow;
    public double lightLevel;

    void Start()
    {
        if (rain)
        {
            GameObject rainPrefab = Resources.Load<GameObject>("Models/Rain");
            foreach (Camera cam in FindObjectsOfType<Camera>())
            {
                GameObject rainPrefabInstance = GameObject.Instantiate(rainPrefab, cam.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
                rainPrefabInstance.GetComponent<RainScript>().Camera = cam;
            }
        }
    }

}
