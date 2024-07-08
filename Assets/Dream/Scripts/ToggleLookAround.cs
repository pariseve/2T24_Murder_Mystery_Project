using UnityEngine;
using System.Collections.Generic;

public class ToggleLookAround : MonoBehaviour
{
    [SerializeField] private string[] componentNames; // Array to hold component names

    void Start()
    {
    }

    // Method to set all components in the array to active
    public void EnableComponent()
    {
        SetComponentsActive(true);
    }

    // Method to set all components in the array to inactive
    public void DisableComponent()
    {
        SetComponentsActive(false);
    }

    // Private method to handle the enabling/disabling logic for all components
    private void SetComponentsActive(bool isActive)
    {
        // Find all root GameObjects in the scene
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        // Iterate through each component name in the array
        foreach (string componentName in componentNames)
        {
            // Create a list to hold all found components for the current componentName
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
    }

    void Update()
    {
        /*
        // Toggle all components on key press for demonstration
        if (Input.GetKeyDown(KeyCode.T))
        {
            EnableComponents();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            DisableComponents();
        }
        */
    }
}


