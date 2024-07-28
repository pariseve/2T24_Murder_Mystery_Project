using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowImageUI : MonoBehaviour
{
    [SerializeField] private GameObject[] uiElements; // Array of UI elements
    [SerializeField] private float animationDuration = 0.5f; // Duration for the scaling animation
    [SerializeField] private string[] componentsToDisable; // Names of components to disable/enable

    private Dictionary<string, GameObject> componentReferences = new Dictionary<string, GameObject>();

    void Start()
    {
        // Set initial state for all UI elements
        foreach (var uiElement in uiElements)
        {
            uiElement.transform.localScale = Vector3.zero;
            uiElement.SetActive(false);
        }

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

    public void EnableUI()
    {
        StartCoroutine(AnimateUI(true));
        CheckAndUpdatePlayerMovement();
    }

    public void DisableUI()
    {
        StartCoroutine(AnimateUI(false));
        CheckAndUpdatePlayerMovement();
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
        foreach (var uiElement in uiElements)
        {
            if (uiElement.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator AnimateUI(bool enable)
    {
        foreach (var uiElement in uiElements)
        {
            if (enable)
            {
                uiElement.SetActive(true);
                ToggleComponents(false);
                LeanTween.scale(uiElement, Vector3.one, animationDuration).setEaseOutBounce();
            }
            else
            {
                LeanTween.scale(uiElement, Vector3.zero, animationDuration).setEaseInBounce().setOnComplete(() =>
                {
                    uiElement.SetActive(false);
                    ToggleComponents(true);
                });
            }
        }

        yield return new WaitForSeconds(animationDuration);

        // Call CheckAndUpdatePlayerMovement after the animation has completed
        CheckAndUpdatePlayerMovement();
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