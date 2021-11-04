using System.Collections.Generic;

public interface ISensor
{
    void Start();

    void Run(Dictionary<string, object> args);
}
