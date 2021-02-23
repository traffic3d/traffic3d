using System.Collections;
using Tests;

public abstract class ArrangeActAssertStrategy : EvacuAgentCommonSceneTest
{
    public abstract IEnumerator PerformTest();
    public abstract void Arrange();
    public abstract void Act();
    public abstract IEnumerator Assertion();
}
