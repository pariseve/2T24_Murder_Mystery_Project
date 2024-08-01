using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class AlarmManager : MonoBehaviour
{
    [SerializeField] private DialControl volumeDial;
    [SerializeField] private DialControl frequencyDial;
    [SerializeField] private DialControl durationDial;

    [SerializeField] private GameObject settingsUIPanel;
    [SerializeField] private GameObject setAlarmUIPanel;
    [SerializeField] private Button setAlarmButton;
    [SerializeField] private Button manualOverdriveButton;
    [SerializeField] private Button gotoSetAlarmPanelButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [SerializeField] private GameObject completeText;
    [SerializeField] private AudioSource alarmSound;
    [SerializeField] private TextMeshProUGUI timerText; // Reference to the TextMeshPro component for the timer

    [SerializeField] private string boolName;

    [SerializeField] private float timeLimit = 10f;
    private float timer;
    private bool isAlarmActive;

    [SerializeField] private string[] componentsToDisable; // Names of components to disable/enable

    private Dictionary<string, GameObject> componentReferences = new Dictionary<string, GameObject>();

    void Start()
    {
        setAlarmButton.onClick.AddListener(OnSetAlarmButtonClicked);
        manualOverdriveButton.onClick.AddListener(OnManualOverdriveButtonClicked);
        gotoSetAlarmPanelButton.onClick.AddListener(OnGotoSetAlarmPanelButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);

        SetInteractableUIElements(false);

        // Populate the dictionary with references to the components to disable/enable
        foreach (string componentName in componentsToDisable)
        {
            GameObject foundObject = FindGameObjectByName(componentName);
            if (foundObject != null)
            {
                componentReferences[componentName] = foundObject;
            }
            else
            {
                Debug.LogWarning($"Failed to find {componentName} during initialization.");
            }
        }
    }

    void Update()
    {
        if (IsUIActive())
        {
            CheckAndUpdatePlayerMovement();
        }
        if (isAlarmActive)
        {
            timer -= Time.deltaTime;
            timerText.text = $"Time Left: {timer:F2}"; // Update the timer text
            CheckAndUpdatePlayerMovement();

            if (timer <= 0)
            {
                ResetSettings();
                feedbackText.text = "The system automatically shut down, try again";
                StartCoroutine(AnimateFeedbackText(false)); // Animate feedback text and deactivate it
                isAlarmActive = false;
                timerText.gameObject.SetActive(false); // Hide the timer text when time runs out
                SetInteractableUIElements(false); // Disable UI elements
            }
        }
    }

    private void ToggleComponents(bool enable)
    {
        foreach (var componentName in componentsToDisable)
        {
            GameObject component = FindGameObjectByName(componentName);
            if (component != null)
            {
                component.SetActive(enable);
                Debug.Log($"Set {componentName} active state to {enable}");
            }
            else
            {
                Debug.LogWarning($"Failed to find {componentName}");
            }
        }
    }

    private GameObject FindGameObjectByName(string gameObjectName)
    {
        // Search in current hierarchy
        GameObject foundObject = GameObject.Find(gameObjectName);

        // If not found, search in DontDestroyOnLoad objects
        if (foundObject == null)
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>(true); // Use true to include inactive objects
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == gameObjectName)
                {
                    return obj;
                }
            }
        }

        return foundObject;
    }

    public void SetBoolVariable()
    {
        if (BoolManager.Instance != null)
        {
            BoolManager.Instance.SetBool(boolName, true);
        }
        else
        {
            Debug.LogError("BoolManager.Instance is null.");
        }
    }

    private void CheckAndUpdatePlayerMovement()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            if (IsUIActive())
            {
                playerController.DisableMovement();
            }
            else
            {
                playerController.EnableMovement();
            }
        }
    }

    public bool IsUIActive()
    {
        return settingsUIPanel.activeSelf || setAlarmUIPanel.activeSelf;
    }

    public void StartAlarmSetup()
    {
        timer = timeLimit;
        isAlarmActive = true;
        timerText.gameObject.SetActive(true); // Show the timer text when starting the alarm setup
        SetInteractableUIElements(true); // Enable UI elements
    }

    private void OnSetAlarmButtonClicked()
    {
        float volume = volumeDial.currentValue;
        float frequency = frequencyDial.currentValue;
        float duration = durationDial.currentValue;

        if (volume == 100f && frequency == 43f && duration == 30f)
        {
            StopCoroutine(ShowCompleteText()); // Stop the completion coroutine if it's running
            StartCoroutine(ShowCompleteText()); // Show the complete text and start the success sequence
            timer = 0f;
            isAlarmActive = false;
        }
        else
        {
            feedbackText.text = "Not the correct values, try again";
            StartCoroutine(AnimateFeedbackText(false)); // Animate feedback text and deactivate it
            ResetSettings();
        }
    }

    private void OnManualOverdriveButtonClicked()
    {
        StartAlarmSetup(); // Start the timer and make UI elements interactable
    }

    private void OnGotoSetAlarmPanelButtonClicked()
    {
        settingsUIPanel.SetActive(false);
        setAlarmUIPanel.SetActive(true);
    }

    private void OnBackButtonClicked()
    {
        setAlarmUIPanel.SetActive(false);
        settingsUIPanel.SetActive(true);
    }

    private void ResetSettings()
    {
        volumeDial.currentValue = 0f; // Set default values
        frequencyDial.currentValue = 0f;
        durationDial.currentValue = 0f;
        // Update dials to show default values if needed
        volumeDial.UpdateDial();
        frequencyDial.UpdateDial();
        durationDial.UpdateDial();
    }

    private IEnumerator ShowCompleteText()
    {
        completeText.SetActive(true);
        completeText.transform.localScale = Vector3.zero;
        LeanTween.scale(completeText, Vector3.one, 0.5f).setEaseOutBounce();

        yield return new WaitForSeconds(2f);

        LeanTween.scale(completeText, Vector3.zero, 0.5f).setEaseInBounce().setOnComplete(() =>
        {
            completeText.SetActive(false);
        });

        settingsUIPanel.SetActive(false);
        setAlarmUIPanel.SetActive(false);
        timerText.gameObject.SetActive(false);
        alarmSound.Play();
        DontDestroyOnLoad(alarmSound.gameObject);
        timer = 0f;
        isAlarmActive = false;
        SetBoolVariable();

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.EnableMovement();
        }

        yield return new WaitForSeconds(30f); // Wait for 30 seconds before stopping and destroying the alarm
        StopSoundAndDestroy(); // Stop the sound and destroy the object
    }

    private void StopSoundAndDestroy()
    {
        SetInteractableUIElements(true);
        // Stop the sound if it's still playing
        if (alarmSound.isPlaying)
        {
            alarmSound.Stop();
        }

        // Destroy the game object with the audio source
        Destroy(alarmSound.gameObject);
    }

    private IEnumerator AnimateFeedbackText(bool success)
    {
        feedbackText.gameObject.SetActive(true);
        feedbackText.transform.localScale = Vector3.zero;
        LeanTween.scale(feedbackText.gameObject, Vector3.one, 0.5f).setEaseOutBounce();

        yield return new WaitForSeconds(2f);

        LeanTween.scale(feedbackText.gameObject, Vector3.zero, 0.5f).setEaseInBounce().setOnComplete(() =>
        {
            feedbackText.gameObject.SetActive(false);
        });

        if (success)
        {
            yield return new WaitForSeconds(1f);
            timerText.gameObject.SetActive(false); // Hide timer text on success
        }
    }

    private void SetInteractableUIElements(bool interactable)
    {
        volumeDial.enabled = interactable;
        frequencyDial.enabled = interactable;
        durationDial.enabled = interactable;
        setAlarmButton.interactable = interactable;
        gotoSetAlarmPanelButton.interactable = interactable;
        // backButton.interactable = interactable;
        manualOverdriveButton.interactable = !interactable; // Always keep the manual overdrive button interactable
    }

    public void CloseAllUI()
    {
        // Ensure the alarm is stopped and settings reset
        timer = 0f;
        isAlarmActive = false; // Stop the alarm if it was active
        // settingsUIPanel.SetActive(false);
        // setAlarmUIPanel.SetActive(false);
        completeText.SetActive(false);
        timerText.gameObject.SetActive(false);
        feedbackText.gameObject.SetActive(false);
        // ResetSettings(); // Reset settings to default
        SetInteractableUIElements(false); // Disable UI elements

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.EnableMovement();
        }
    }
}




