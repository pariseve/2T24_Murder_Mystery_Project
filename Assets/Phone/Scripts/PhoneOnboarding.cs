using UnityEngine;
using DialogueEditor;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class OnboardingStep
{
    public NPCConversation dialogue; // Dialogue for the step
    public string applicationIconName; // Name of the application icon to highlight during this step
}

public class PhoneOnboarding : MonoBehaviour
{
    [SerializeField] private OnboardingStep[] onboardingSteps; // Array of onboarding steps
    [SerializeField] private NPCConversation startingDialogue; // Dialogue to show when onboarding starts
    [SerializeField] private NPCConversation completionDialogue; // Final dialogue to show when onboarding is complete
    [SerializeField] private string onboardingTriggeredKey = "PhoneOnboardingTriggered";
    [SerializeField] private string onboardingBoolKey = "StartOnboarding";
    private Dictionary<string, GameObject> applicationIcons = new Dictionary<string, GameObject>(); // Store references to application icons

    private int currentStepIndex = 0;
    [SerializeField] private bool isOnboardingTriggered = false;
    [SerializeField] private bool onboardingComplete = false;
    private PhoneSystem phoneSystem;

    void Start()
    {
        // Find the PhoneSystem instance
        phoneSystem = FindObjectOfType<PhoneSystem>();

        // Check if the onboarding has already been triggered
        if (PlayerPrefs.HasKey(onboardingTriggeredKey))
        {
            isOnboardingTriggered = true;
            onboardingComplete = true;
            currentStepIndex = onboardingSteps.Length; // Set to the end to skip onboarding
        }
        else
        {
            // Cache application icons
            CacheApplicationIcons();

            // Start a coroutine to wait for the boolean to be true
            StartCoroutine(WaitForOnboardingBool());
        }
    }

    private IEnumerator WaitForOnboardingBool()
    {
        // Wait until the boolean from BoolManager is true
        while (!BoolManager.Instance.GetBool(onboardingBoolKey))
        {
            yield return null; // Wait for the next frame
        }

        // Wait until the DialoguePanel is not active
        while (ConversationManager.Instance.DialoguePanel.gameObject.activeInHierarchy)
        {
            yield return null; // Wait for the next frame
        }

        if (!isOnboardingTriggered && !onboardingComplete)
        {
            phoneSystem.isPhoneVisible = true;
            phoneSystem.SlidePhone();
            // Wait until the phone is fully visible
            yield return StartCoroutine(WaitForPhoneToSlide());
            StartOnboarding();
        }
    }

    private IEnumerator WaitForPhoneToSlide()
    {
        while (!phoneSystem.IsPhoneVisible())
        {
            yield return null; // Wait for the next frame
        }
    }


    void Update()
    {
        if (isOnboardingTriggered && !onboardingComplete)
        {
            // Disable player movement or any other setup needed before dialogue
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.DisableMovement();
            }
            CacheApplicationIcons();
            HighlightCurrentApplicationIcon();
        }
    }

    public void OnIconClicked(string iconName)
    {
        if (isOnboardingTriggered)
        {
            Debug.Log($"Icon Clicked: {iconName}, Current Step: {currentStepIndex}");

            if (iconName == onboardingSteps[currentStepIndex].applicationIconName)
            {
                Debug.Log("Correct icon clicked, starting dialogue.");
                StartDialogueForCurrentStep();
            }
            else
            {
                Debug.Log("Incorrect icon clicked.");
            }
        }
    }

    private void CacheApplicationIcons()
    {
        foreach (OnboardingStep step in onboardingSteps)
        {
            GameObject icon = FindObjectByName(step.applicationIconName);
            if (icon != null)
            {
                applicationIcons[step.applicationIconName] = icon;
                Debug.Log($"Icon cached: {step.applicationIconName}");
            }
            else
            {
                Debug.LogWarning($"Icon not found: {step.applicationIconName}");
            }
        }
    }

    private void HighlightCurrentApplicationIcon()
    {
        Debug.Log($"Highlighting icon for step {currentStepIndex}: {onboardingSteps[currentStepIndex].applicationIconName}");
        foreach (var iconEntry in applicationIcons)
        {
            bool isHighlighted = iconEntry.Key == onboardingSteps[currentStepIndex].applicationIconName;
            iconEntry.Value.SetActive(isHighlighted);
            Debug.Log($"{iconEntry.Key} active: {isHighlighted}");
        }
    }


    private void StartOnboarding()
    {
        if (startingDialogue != null)
        {
            // Check if DialoguePanel is active
            if (ConversationManager.Instance.DialoguePanel.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("Cannot start a new conversation while the dialogue panel is active.");
                return;
            }

            // Start the initial dialogue
            Debug.Log("Starting onboarding with initial dialogue.");
            ConversationManager.Instance.StartConversation(startingDialogue);
            isOnboardingTriggered = true;

            // Disable the phone toggle during onboarding
            // phoneSystem.DisablePhoneToggle();
            StartCoroutine(DisableThePhoneToggle());

            // Highlight the first onboarding step icon
            HighlightCurrentApplicationIcon();
        }
    }

    private IEnumerator DisableThePhoneToggle()
    {
        yield return new WaitForSeconds(0.1f);

        phoneSystem.DisablePhoneToggle();

    }

    private void StartDialogueForCurrentStep()
    {
        if (currentStepIndex < onboardingSteps.Length)
        {
            Debug.Log($"Starting dialogue for step {currentStepIndex}: {onboardingSteps[currentStepIndex].dialogue.name}");
            // Start the conversation for the current step
            ConversationManager.Instance.StartConversation(onboardingSteps[currentStepIndex].dialogue);

            // Add a callback to progress onboarding after the dialogue finishes
            ConversationManager.Instance.OnDialogueEnd += OnDialogueEndForCurrentStep;
        }
        else
        {
            Debug.Log("No more onboarding steps to trigger.");
            // Show final completion dialogue if available
            if (completionDialogue != null)
            {
                Debug.Log("Starting completion dialogue.");
                ConversationManager.Instance.StartConversation(completionDialogue);
                ConversationManager.Instance.OnDialogueEnd += OnDialogueComplete;
            }
            else
            {
                // Complete onboarding if no final dialogue is set
                CompleteOnboarding();
            }
        }
    }

    private void OnDialogueEndForCurrentStep()
    {
        // Unsubscribe from the event to avoid multiple calls
        ConversationManager.Instance.OnDialogueEnd -= OnDialogueEndForCurrentStep;

        Debug.Log("Dialogue ended, progressing onboarding.");
        ProgressOnboarding();
    }

    private void OnDialogueComplete()
    {
        // Unsubscribe from the event to avoid multiple calls
        ConversationManager.Instance.OnDialogueEnd -= OnDialogueComplete;

        Debug.Log("Completion dialogue ended.");
        CompleteOnboarding();
    }

    private void ProgressOnboarding()
    {
        if (currentStepIndex >= onboardingSteps.Length)
        {
            // Finish onboarding if no more steps
            Debug.Log("Onboarding complete.");
            CompleteOnboarding();
            return;
        }

        // Disable the current application's icon highlight
        Debug.Log($"Disabling highlight for icon: {onboardingSteps[currentStepIndex].applicationIconName}");
        HighlightApplicationIcon(onboardingSteps[currentStepIndex].applicationIconName, false);

        currentStepIndex++;

        if (currentStepIndex < onboardingSteps.Length)
        {
            // Trigger the next onboarding step
            HighlightCurrentApplicationIcon();
        }
        else
        {
            // Finish onboarding if no more steps
            Debug.Log("No more onboarding steps.");
            StartDialogueForCurrentStep(); // This will trigger the completion dialogue if set
        }
    }

    private void CompleteOnboarding()
    {
        // Re-enable the phone toggle functionality
        phoneSystem.EnablePhoneToggle();
        onboardingComplete = true;

        // Re-enable all application icons
        foreach (var iconEntry in applicationIcons)
        {
            // Set all icons back to active
            iconEntry.Value.SetActive(true);
        }

        // Set onboarding to complete
        PlayerPrefs.SetInt(onboardingTriggeredKey, 1);
        PlayerPrefs.Save();

        Debug.Log("Onboarding process completed.");

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.EnableMovement();
        }
    }

    private void HighlightApplicationIcon(string iconName, bool highlight = true)
    {
        if (applicationIcons.TryGetValue(iconName, out GameObject icon))
        {
            // Toggle the icon's active state
            icon.SetActive(highlight);
        }
    }

    private GameObject FindObjectByName(string name)
    {
        // Check the current scene's active objects
        GameObject foundObject = GameObject.Find(name);
        if (foundObject != null)
            return foundObject;

        // Check the DontDestroyOnLoad objects
        foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            foundObject = FindInHierarchy(root, name);
            if (foundObject != null)
                return foundObject;
        }

        return null;
    }

    private GameObject FindInHierarchy(GameObject root, string name)
    {
        // Check if the root itself is the object we are looking for
        if (root.name == name)
            return root;

        // Recursively check all children
        foreach (Transform child in root.transform)
        {
            GameObject found = FindInHierarchy(child.gameObject, name);
            if (found != null)
                return found;
        }

        return null;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed
        if (ConversationManager.Instance != null)
        {
            ConversationManager.Instance.OnDialogueEnd -= OnDialogueEndForCurrentStep;
            ConversationManager.Instance.OnDialogueEnd -= OnDialogueComplete;
        }
    }
}

