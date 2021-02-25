using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private Text timerTextComponent;

    [SerializeField]
    private GameObject menuSimulationPanel;

    [SerializeField]
    private GameObject menuInformationPanel;

    [SerializeField]
    private GameObject menuPanelParent;

    [SerializeField]
    private Animator menuPanelParentAnimator;

    [SerializeField]
    private Text NumberOfPedestriansText;

    [SerializeField]
    private Text NumberOfShootersText;

    private GameObject currentMenuContentPanel;
    private SimulationDataTracker simulationDataTracker;
    private const string isMenuMinimisedAnimatorString = "isMenuMinimised";
    private const string shooterTag = "shooter";

    private void Start()
    {
        simulationDataTracker = GameObject.FindObjectOfType<SimulationDataTracker>();
        currentMenuContentPanel = menuSimulationPanel;
        currentMenuContentPanel.SetActive(true);
    }

    void Update()
    {
        timerTextComponent.text = Time.timeSinceLevelLoad.ToString();

        if (currentMenuContentPanel.Equals(menuInformationPanel))
        {
            UpdateInformationPanel();
        }
    }

    public void OnButtonClickChangeMenuContentState(int menuContentState)
    {
        currentMenuContentPanel.SetActive(false);

        switch ((MenuContentState)menuContentState)
        {
            case MenuContentState.SimulationMenuContent: currentMenuContentPanel = menuSimulationPanel;
                break;
            case MenuContentState.InformationMenuContent:
                currentMenuContentPanel = menuInformationPanel;
                break;
        }

        currentMenuContentPanel.SetActive(true);
        OpenMinimisedMenuOnMenuTabClick();
    }

    public void MinimiseMaximiseMenu()
    {
        bool isMenuMinimised = menuPanelParentAnimator.GetBool(isMenuMinimisedAnimatorString);
        menuPanelParentAnimator.SetBool(isMenuMinimisedAnimatorString, !isMenuMinimised);
    }

    private void OpenMinimisedMenuOnMenuTabClick()
    {
        bool isMenuMinimised = menuPanelParentAnimator.GetBool(isMenuMinimisedAnimatorString);

        if (isMenuMinimised)
        {
            menuPanelParentAnimator.SetBool(isMenuMinimisedAnimatorString, !isMenuMinimised);
        }
    }

    private void UpdateInformationPanel()
    {
        NumberOfPedestriansText.text = simulationDataTracker.GetNumberOfType(typeof(Pedestrian)).ToString();
        NumberOfShootersText.text = simulationDataTracker.GetNumberOfObjectsWithTag(shooterTag).ToString();
    }
}

public enum MenuContentState
{
    SimulationMenuContent = 0,
    InformationMenuContent = 1
}
