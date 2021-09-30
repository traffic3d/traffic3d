using UnityEngine;

public class SnowEnvironmentState : IEnvironmentState
{
    public const int snowHeight = 50;
    public const int snowLengthForward = 40;

    public void EnableEnvironmentState(EnvironmentSettings environmentSettings)
    {
        environmentSettings.SetSurfaceMaterial(environmentSettings.snowSurfaceMaterial);
        GameObject snowPrefab = Resources.Load<GameObject>("Models/Snow");
        foreach (Camera cam in GameObject.FindObjectsOfType<Camera>())
        {
            Vector3 camPosition = cam.transform.position;
            GameObject snowPrefabInstance = GameObject.Instantiate(snowPrefab, new Vector3(camPosition.x, camPosition.y + snowHeight, camPosition.z) + cam.transform.forward * snowLengthForward, Quaternion.Euler(new Vector3(0, 0, 0)));
        }
        foreach (Renderer renderer in GameObject.FindObjectsOfType<Renderer>())
        {
            if (renderer.material.name.ToLower().Contains("grass"))
            {
                renderer.material = environmentSettings.snowMaterial;
            }
        }
        RenderSettings.skybox = environmentSettings.snowSkyBox;
    }

    public EnvironmentStateType GetEnvironmentStateType()
    {
        return EnvironmentStateType.SNOW;
    }

    public Priority GetPriority()
    {
        return Priority.MEDIUM;
    }
}
