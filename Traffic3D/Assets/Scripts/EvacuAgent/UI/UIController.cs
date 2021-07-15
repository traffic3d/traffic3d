using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject currentMenuContentPanel;

    [SerializeField]
    private TextMeshProUGUI timerTextComponent;

    [SerializeField]
    private GameObject menuSimulationPanel;

    [SerializeField]
    private GameObject menuInformationPanel;

    [SerializeField]
    private GameObject legendPanel;

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
        float roundedTimeValue = Mathf.Round(Time.timeSinceLevelLoad * 100f) / 100f;
        timerTextComponent.text = roundedTimeValue.ToString();
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
            case MenuContentState.LegendMenuContent:
                currentMenuContentPanel = legendPanel;
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

    public void UpdateSimulationPlaySpeed(float speedValue)
    {
        Time.timeScale = speedValue;
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
    InformationMenuContent = 1,
    LegendMenuContent = 2
}
