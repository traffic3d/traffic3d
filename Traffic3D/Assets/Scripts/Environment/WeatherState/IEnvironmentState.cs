using System;

public interface IEnvironmentState
{
    /// <summary>
    /// Enables the weather state and sets the scene.
    /// </summary>
    /// <param name="environmentSettings">The scenes environment settings</param>
    void EnableEnvironmentState(EnvironmentSettings environmentSettings);

    /// <summary>
    /// Gets the type of state.
    /// </summary>
    /// <returns>The Environment State Type</returns>
    EnvironmentStateType GetEnvironmentStateType();

    /// <summary>
    /// Gets the priority of the weather state and will enable in order of priority.
    /// </summary>
    /// <returns>The priority</returns>
    Priority GetPriority();
}

[Flags]
public enum EnvironmentStateType
{
    RAIN,
    SNOW,
    NIGHT
}
