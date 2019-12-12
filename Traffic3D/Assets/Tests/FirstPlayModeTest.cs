using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;

[Category("Tests")]
public class FirstPlayModeTest
{
    [Test]
    public void FirstPlayModeTestSimplePasses()
    {
        // Use the Assert class to test conditions.
        Assert.AreEqual(true, true);
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator FirstPlayModeTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
}
