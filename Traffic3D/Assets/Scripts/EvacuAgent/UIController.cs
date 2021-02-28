using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject currentMenuContentPanel;

    [SerializeField]
    private Text timerTextComponent;

    [SerializeField]
    private GameObject menuSimulationPanel;

    [SerializeField]
    private GameObject menuInformationPanel;

    [SerializeField]
    private Animator menuPanelParentAnimator;

    [SerializeField]
    private Text NumberOfShootersText;

    private const string isMenuMinimisedAnimatorString = "isMenuMinimised";

    private void Start()
    {
        currentMenuContentPanel = menuSimulationPanel;
        currentMenuContentPanel.SetActive(true);
    }

    void Update()
    {
        timerTextComponent.text = Time.timeSinceLevelLoad.ToString();
    }

    public void OnButtonClickChangeMenuContentState(int menuContentState)
    {
        currentMenuContentPanel.SetActive(false);

        switch ((MenuContentState)menuContentState)
        {
            default:
            case MenuContentState.SimulationMenuContent:
                currentMenuContentPanel = menuSimulationPanel;
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
}

public enum MenuContentState
{
    SimulationMenuContent = 0,
    InformationMenuContent = 1
}
