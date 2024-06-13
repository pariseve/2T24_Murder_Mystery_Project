using UnityEngine;
using System.Collections.Generic;

public class ToggleLookAround : MonoBehaviour
{
    public string componentName;

    void Start()
    {
    }

    // Method to set the component to active
    public void EnableComponent()
    {
        SetComponentActive(true);
    }

    // Method to set the component to inactive
    public void DisableComponent()
    {
        SetComponentActive(false);
    }

    // Private method to handle the enabling/disabling logic
    private void SetComponentActive(bool isActive)
    {
        // Find all root GameObjects in the scene
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        // Create a list to hold all found components
        List<MonoBehaviour> foundComponents = new List<MonoBehaviour>();

        // Iterate through all root objects and their children
        foreach (GameObject rootObject in rootObjects)
        {
            foundComponents.AddRange(rootObject.GetComponentsInChildren<MonoBehaviour>(true));
        }

        // Enable or disable the specified component
        foreach (MonoBehaviour component in foundComponents)
        {
            if (component.GetType().Name == componentName)
            {
                component.enabled = isActive;
            }
        }
    }

    void Update()
    {
        // Toggle the component on key press for demonstration
        if (Input.GetKeyDown(KeyCode.T))
        {
            EnableComponent();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            DisableComponent();
        }
    }
}
