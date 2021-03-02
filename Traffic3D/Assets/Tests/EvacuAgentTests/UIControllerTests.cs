using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UIController_OnButtonClickChangeMenuContentState_SetsTheCurrentStateToInformationMenuContentCorrectly : ArrangeActAssertStrategy
{
    private UIController uIController;
    private GameObject initialMenuContentPanel;
    private GameObject actualMenuContentPanel;
    private int menuContentState;

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
        uIController = GameObject.FindObjectOfType<UIController>();
        initialMenuContentPanel = uIController.currentMenuContentPanel;
        menuContentState = 1;
    }

    public override void Act()
    {
        uIController.OnButtonClickChangeMenuContentState(menuContentState);
        actualMenuContentPanel = uIController.currentMenuContentPanel;
    }

    public override void Assertion()
    {
        Assert.AreNotSame(initialMenuContentPanel, actualMenuContentPanel);
    }
}

public class UIController_OnButtonClickChangeMenuContentState_SetsTheCurrentStateToSimulationMenuContentCorrectly : ArrangeActAssertStrategy
{
    private UIController uIController;
    private GameObject initialMenuContentPanel;
    private GameObject actualMenuContentPanel;
    private int menuContentState;

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
        uIController = GameObject.FindObjectOfType<UIController>();
        initialMenuContentPanel = uIController.currentMenuContentPanel;
        menuContentState = 0;
    }

    public override void Act()
    {
        uIController.OnButtonClickChangeMenuContentState(menuContentState);
        actualMenuContentPanel = uIController.currentMenuContentPanel;
    }

    public override void Assertion()
    {
        Assert.AreSame(initialMenuContentPanel, actualMenuContentPanel);
    }
}

public class UIController_OnButtonClickChangeMenuContentState_SetsTheCurrentStateToSimulationMenuContentByDefault : ArrangeActAssertStrategy
{
    private UIController uIController;
    private GameObject initialMenuContentPanel;
    private GameObject actualMenuContentPanel;
    private int menuContentState;

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
        uIController = GameObject.FindObjectOfType<UIController>();
        initialMenuContentPanel = uIController.currentMenuContentPanel;
        menuContentState = 3;
    }

    public override void Act()
    {
        uIController.OnButtonClickChangeMenuContentState(menuContentState);
        actualMenuContentPanel = uIController.currentMenuContentPanel;
    }

    public override void Assertion()
    {
        Assert.AreSame(initialMenuContentPanel, actualMenuContentPanel);
    }
}
