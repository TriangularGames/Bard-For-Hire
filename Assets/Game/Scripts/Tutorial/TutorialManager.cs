using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header ("UI Stuff")]
    [SerializeField] private GameObject tutorialPanel; // Main panel
    [SerializeField] private TMP_Text tutorialText; // Text for tutorial
    [SerializeField] private Button tutorialButton; // Button to continue
    private GameObject currentHighlight; // Current highlight
    [SerializeField] private bool doTutorial = false; // Make me do the tutorial
    public bool IsTutorialActive; // Is we doin the tutorial
    public bool IsWaitingForAction; // Waiting for a task completion

    private Queue<TutorialStep> tutorialSteps = new Queue<TutorialStep>(); // Steps of the tutorial
    private TutorialStep currentStep; // What step we on
    private System.Action stepComplete; // We completed a step?

    private const string TutorialKey = "TutorialComplete"; // PlayerPrefs store have we completed tutorial

    public static TutorialManager Instance;

    private void Awake() // Set up things
    {
        Instance = this;
        if (!WeShowTutorial())
        {
            if (tutorialPanel != null) Destroy(tutorialPanel);
            Destroy(gameObject);
            return;
        }
            tutorialPanel.SetActive (false);
        tutorialButton.onClick.AddListener(onContinuePressed);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public bool WeShowTutorial() // SHould we show the tutorial? If player doesn't have it in PlayerPrefs or if we force tutorial
    {
        Debug.Log("Key exists: " + PlayerPrefs.HasKey(TutorialKey));
        Debug.Log("Value is: " + PlayerPrefs.GetInt(TutorialKey, 0));
        return doTutorial || (PlayerPrefs.GetInt(TutorialKey) == 0) || !PlayerPrefs.HasKey(TutorialKey);
    }

    public void ShowTutorial(List<TutorialStep> steps, System.Action onComplete = null) // Starts the tutorial for the player
    {
        IsTutorialActive = true;
        tutorialSteps = new Queue<TutorialStep> (steps);
        stepComplete = onComplete;
        ShowNextStep();
    }

    public void HidePanel() // Hide tutorial panel
    {
        tutorialPanel.SetActive(false);
    }

    public void ShowPanel() // Show tutorial panel
    {
        if (!IsTutorialActive) return;
        tutorialPanel.SetActive(true);
    }

    public void ShowNextStep() // Moves onto next step in tutorial
    {
        if(tutorialSteps.Count == 1) // End tutorial if we are on the last step
        {
            EndTutorial();
            Debug.Log("Tutorial Completed");
            return;
        }
        if (currentHighlight != null) // No current higlight if nothing to highlight
            currentHighlight.SetActive (false);
        currentStep = tutorialSteps.Dequeue (); // Dequeue current step
        tutorialPanel.SetActive (true); 
        tutorialText.text = currentStep.message; // Set the message to the message of the current step

        if (currentStep.highlight != null) // Set the highlight if something needs to be highlighted
        {
            currentHighlight = currentStep.highlight;
            currentHighlight.SetActive (true);
        }
        else
        {
            currentHighlight = null;
        }

        IsWaitingForAction = currentStep.waitForAction; // Watiing for action by player
        tutorialButton.gameObject.SetActive(!currentStep.waitForAction);
        if (currentStep.onEnter != null) // Enters current step
        {
            currentStep.onEnter();
        }
    }

    public void ActionComplete() // The player has done required acttion and can move to next step
    {
        if (!IsTutorialActive || !IsWaitingForAction) return;
        IsWaitingForAction = false;
        if (currentStep.onActionComplete != null)
        {
            currentStep.onActionComplete();
        }
        ShowNextStep ();
    }

    private void onContinuePressed() // Player has pressed continue button so they can move on
    {
        if (IsWaitingForAction || !IsTutorialActive) return;
        if (currentStep.onContinue != null)
        {
            currentStep.onContinue();
        }
        ShowNextStep () ;
    }

    public void EndTutorial() // Ends tutorial and it's functionality, saves playerpref
    {
        IsTutorialActive = false;
        tutorialPanel.SetActive (false);
        PlayerPrefs.SetInt(TutorialKey, 1);
        PlayerPrefs.Save();
        if (stepComplete != null)
        {
            stepComplete();
        }
    }

    public void Advance() // Show next step
    {
        if (!IsTutorialActive) return;
        IsWaitingForAction = false;
        ShowNextStep();
    }
}

[System.Serializable]
public class TutorialStep // The different steps of the tutorial
{
    public string message; // Text shown
    public GameObject highlight; // Highlighted part
    public bool waitForAction; // Waiting for the action
    public System.Action onEnter; // Begin the task
    public System.Action onContinue; // Continue button pressed on dialogue
    public System.Action onActionComplete; // Action is completed
}