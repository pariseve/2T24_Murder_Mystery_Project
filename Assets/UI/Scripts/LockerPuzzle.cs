using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class LockerPuzzle : MonoBehaviour
{
    [SerializeField] private Image[] lockers; // Assign all locker images in the inspector
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private TMP_Text completeText;
    [SerializeField] private string[] clues;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private GameObject[] uiElements; // Assign all UI elements to be animated
    [SerializeField] private float animationDuration = 0.5f;

    private int currentClueIndex = 0;
    private int correctLockerIndex = 11; // Index of locker 42 (3rd row, 2nd column in a 0-based index array)

    [SerializeField] private string[] componentsToDisable; // Names of components to disable/enable

    private Dictionary<string, GameObject> componentReferences = new Dictionary<string, GameObject>();

    [SerializeField] private string boolName;

    void Start()
    {
        // Initialize lockers with click events
        for (int i = 0; i < lockers.Length; i++)
        {
            int index = i;
            AddClickEventToLocker(lockers[i], index);
        }
        // Show the first clue
        ShowCurrentClue();

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

    void AddClickEventToLocker(Image locker, int index)
    {
        EventTrigger trigger = locker.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((eventData) => { OnLockerClick(index); });
        trigger.triggers.Add(entry);
    }

    void OnLockerClick(int index)
    {
        StopAllCoroutines(); // Stop any ongoing animations
        if (index == correctLockerIndex)
        {
            StartCoroutine(ShowCompleteText());
        }
        else
        {
            feedbackText.text = "The key doesn't fit, try again.";
            StartCoroutine(AnimateFeedbackText());
        }
    }

    void ShowCurrentClue()
    {
        feedbackText.text = clues[currentClueIndex];
        feedbackText.gameObject.SetActive(true);
        StartCoroutine(AnimateClueText());
    }

    private IEnumerator AnimateFeedbackText()
    {
        feedbackText.transform.localScale = Vector3.zero;
        LeanTween.scale(feedbackText.gameObject, Vector3.one, animationDuration).setEaseOutBounce();

        yield return new WaitForSeconds(2f);

        LeanTween.scale(feedbackText.gameObject, Vector3.zero, animationDuration).setEaseInBounce().setOnComplete(() =>
        {
            feedbackText.gameObject.SetActive(false);

            // Show the next clue
            if (currentClueIndex < clues.Length - 1)
            {
                currentClueIndex++;
            }
            ShowCurrentClue();
        });
    }

    private IEnumerator AnimateClueText()
    {
        feedbackText.transform.localScale = Vector3.zero;
        LeanTween.scale(feedbackText.gameObject, Vector3.one, animationDuration).setEaseOutBounce();

        yield return null;
    }

    private IEnumerator ShowCompleteText()
    {
        completeText.gameObject.SetActive(true);
        completeText.transform.localScale = Vector3.zero;
        LeanTween.scale(completeText.gameObject, Vector3.one, animationDuration).setEaseOutBounce();

        yield return new WaitForSeconds(2f);

        LeanTween.scale(completeText.gameObject, Vector3.zero, animationDuration).setEaseInBounce().setOnComplete(() =>
        {
            completeText.gameObject.SetActive(false);
            StartCoroutine(AnimateUI(false)); // Close UI panel with animation
        });

        yield return new WaitForSeconds(animationDuration);
    }

    public void DisableUI()
    {
        StartCoroutine(AnimateUI(false));
    }

    private IEnumerator AnimateUI(bool enable)
    {
        if (enable)
        {
            foreach (var uiElement in uiElements)
            {
                uiElement.SetActive(true);
                LeanTween.scale(uiElement, Vector3.one, animationDuration).setEaseOutBounce();
            }
            ToggleComponents(false);
        }
        else
        {
            foreach (var uiElement in uiElements)
            {
                LeanTween.scale(uiElement, Vector3.zero, animationDuration).setEaseInBounce().setOnComplete(() =>
                {
                    uiElement.SetActive(false);
                });
            }
            yield return new WaitForSeconds(animationDuration);
            ToggleComponents(true);
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.EnableMovement();
            }
            if (boolName != null)
            {
                SetBoolVariable();
            }
        }
        yield return null;
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
}


