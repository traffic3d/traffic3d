using NUnit.Framework;
using System.Collections;
using Tests;

[Category("Tests")]
public abstract class ArrangeActAssertStrategy : EvacuAgentCommonSceneTest
{
    public abstract IEnumerator PerformTest();
    public abstract void Arrange();
    public abstract void Act();
    public abstract void Assertion();
}
