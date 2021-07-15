using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class InformationPanelController_UpdateNumberOfPedestrianText_GetsCorrectNumberOfPedestrians : ArrangeActAssertStrategy
{
    private InformationPanelController informationPanelController;
    private string expectedNumberOfPedestrians;
    private List<string> tags;

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
        informationPanelController = InformationPanelControllerTestsHelper.GetInformationPanelController();
        InformationPanelControllerTestsHelper.ClearAllTextFields(informationPanelController);

        tags = new List<string> { InformationPanelControllerTestsHelper.untaggedString, InformationPanelControllerTestsHelper.untaggedString, EvacuAgentSceneParamaters.SHOOTER_TAG };
        expectedNumberOfPedestrians = "3";

        foreach(string tag in tags)
        {
            SpawnGameObjectWithInactivePedestrianScript(tag);
        }
    }

    public override void Act()
    {
        informationPanelController.UpdateNumberOfPedestrianText();
    }

    public override void Assertion()
    {
        StringAssert.IsMatch(expectedNumberOfPedestrians, informationPanelController.NumberOfPedestriansText.text);
    }
}

public class InformationPanelController_GetNumberOfObjectsWithTag_GetsCorrectNumberOfShooter : ArrangeActAssertStrategy
{
    private InformationPanelController informationPanelController;
    private string expectedNumberOfShooter;
    private List<string> tags;

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
        informationPanelController = InformationPanelControllerTestsHelper.GetInformationPanelController();
        InformationPanelControllerTestsHelper.ClearAllTextFields(informationPanelController);

        tags = new List<string> { InformationPanelControllerTestsHelper.untaggedString, InformationPanelControllerTestsHelper.untaggedString, EvacuAgentSceneParamaters.SHOOTER_TAG };
        expectedNumberOfShooter = "1";

        foreach (string tag in tags)
        {
            SpawnGameObjectWithInactivePedestrianScript(tag);
        }
    }

    public override void Act()
    {
        informationPanelController.UpdateNumberOfShootersText();
    }

    public override void Assertion()
    {
        StringAssert.IsMatch(expectedNumberOfShooter, informationPanelController.NumberOfShootersText.text);
    }
}

public static class InformationPanelControllerTestsHelper
{
    public static string untaggedString = "Untagged";

    public static InformationPanelController GetInformationPanelController()
    {
        return GameObject.FindObjectOfType<InformationPanelController>();
    }

    public static void ClearAllTextFields(InformationPanelController informationPanelController)
    {
        foreach(Text textField in informationPanelController.GetComponentsInChildren<Text>())
        {
            textField.text = string.Empty;
        }
    }
}
