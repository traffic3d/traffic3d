using System;
using System.Collections;
using NUnit.Framework;
using Tests;
using UnityEngine.TestTools;


[Category("Tests")]
public class CriteriaValuesTests : EvacuAgentCommonSceneTest
{
    private const string footfall = "footfall";
    private const string distance = "distance";
    private const string invalidString = "invalidString";

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        yield return null;
    }

    [UnityTest]
    public IEnumerator CriteriaValues_GetCriteriaValueFromName_ReturnsCorrectIndexForFootfall()
    {
        // Arrange
        yield return null;
        DisableLoops();

        // Act
        bool actualBool = CriteriaValues.GetCriteriaValueFromName(footfall);

        // Assert
        Assert.IsTrue(actualBool);
    }

    [UnityTest]
    public IEnumerator CriteriaValues_GetCriteriaValueFromName_ReturnsCorrectIndexForDistance()
    {
        // Arrange
        yield return null;
        DisableLoops();

        // Act
        bool actualBool = CriteriaValues.GetCriteriaValueFromName(distance);

        // Assert
        Assert.IsFalse(actualBool);
    }

    [UnityTest]
    public IEnumerator CriteriaValues_GetCriteriaValueFromName_ThrowsException_WhenInputStringIsInvalid()
    {
        // Arrange
        yield return null;
        DisableLoops();

        // Assert
        Assert.Throws<ArgumentException>(() => CriteriaValues.GetCriteriaValueFromName(invalidString));
    }
}
