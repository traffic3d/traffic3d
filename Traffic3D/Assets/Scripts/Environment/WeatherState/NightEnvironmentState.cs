using UnityEngine;

public class NightEnvironmentState : IEnvironmentState
{
    private const float nightLightIntensityConstant = 0.1F;

    public void EnableEnvironmentState(EnvironmentSettings environmentSettings)
    {
        RenderSettings.skybox = environmentSettings.nightSkyBox;
        if (environmentSettings.mainLight != null)
        {
            environmentSettings.mainLight.intensity = environmentSettings.mainLight.intensity * nightLightIntensityConstant;
            environmentSettings.mainLight.color = environmentSettings.nightLightColour;
        }
    }

    public EnvironmentStateType GetEnvironmentStateType()
    {
        return EnvironmentStateType.NIGHT;
    }

    public Priority GetPriority()
    {
        return Priority.HIGH;
    }
}
