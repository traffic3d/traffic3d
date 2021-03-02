using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class InformationPanelController_UpdateNumberOfPedestrianText_GetsCorrectNumberOfPedestrians : ArrangeActAssertStrategy
{
    private InformationPanelController informationPanelController;
    private string expectedNumberOfPedestrians;

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
        expectedNumberOfPedestrians = "2";
        EvacuAgentSceneParamaters.NUMBER_OF_SHOOTER_AGENTS = 1;
        SpawnPedestrians(3);
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
    private Pedestrian[] pedestrians;

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
        expectedNumberOfShooter = "1";
        EvacuAgentSceneParamaters.NUMBER_OF_SHOOTER_AGENTS = 1;
        pedestrians = SpawnPedestrians(3);
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
    public static InformationPanelController GetInformationPanelController()
    {
        UIController uIController = GameObject.FindObjectOfType<UIController>();
        uIController.GetComponentInChildren<InformationPanelController>().gameObject.SetActive(true);
        return uIController.GetComponentInChildren<InformationPanelController>();
    }

    public static void ClearAllTextFields(InformationPanelController informationPanelController)
    {
        foreach(Text textField in informationPanelController.GetComponentsInChildren<Text>())
        {
            textField.text = string.Empty;
        }
    }
}
