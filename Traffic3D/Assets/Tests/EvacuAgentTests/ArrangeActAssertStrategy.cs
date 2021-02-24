using System.Collections;
using Tests;
using UnityEngine.TestTools;

public abstract class ArrangeActAssertStrategy : EvacuAgentCommonSceneTest
{
    public abstract IEnumerator PerformTest();
    public abstract void Arrange();
    public abstract void Act();
    public abstract void Assertion();
}
