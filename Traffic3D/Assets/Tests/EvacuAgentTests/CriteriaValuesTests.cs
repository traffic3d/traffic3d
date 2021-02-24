using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

public class CriteriaValues_GetCriteriaValueFromName_ReturnsCorrectIndexForFootfall : ArrangeActAssertStrategy
{
    private bool actualBool;
    private string footfall;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        yield return null;
        Arrange();
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        DisableLoops();
        actualBool = false;
        footfall = "footfall";
    }

    public override void Act()
    {
        actualBool = CriteriaValues.GetCriteriaValueFromName(footfall);
    }

    public override void Assertion()
    {
        Assert.IsTrue(actualBool);
    }
}

public class CriteriaValues_GetCriteriaValueFromName_ReturnsCorrectIndexForDistance: ArrangeActAssertStrategy
{
    private bool actualBool;
    private string distance;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        yield return null;
        Arrange();
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        DisableLoops();
        actualBool = false;
        distance = "distance";
    }

    public override void Act()
    {
        actualBool = CriteriaValues.GetCriteriaValueFromName(distance);
    }

    public override void Assertion()
    {
        Assert.IsFalse(actualBool);
    }
}

public class CriteriaValues_GetCriteriaValueFromName_ThrowsExceptionForInvalidString : ArrangeActAssertStrategy
{
    private string invalidString;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        yield return null;
        Arrange();
        Assertion();
    }

    public override void Arrange()
    {
        DisableLoops();
        invalidString = "invalidString";
    }

    public override void Act() {}

    public override void Assertion()
    {
        Assert.Throws<ArgumentException>(() => CriteriaValues.GetCriteriaValueFromName(invalidString));
    }
}
