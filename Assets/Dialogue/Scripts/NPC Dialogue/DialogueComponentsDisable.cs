using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DialogueEditor;

public class DialogueComponentsDisable : MonoBehaviour
{
    [SerializeField] private string[] componentsToDisable; // Names of components to disable/enable

    private Dictionary<string, GameObject> componentReferences = new Dictionary<string, GameObject>();

    public static DialogueComponentsDisable Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Register for the sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Populate the dictionary initially
        PopulateComponentReferences();
    }

    void Update()
    {
        bool isDialogueActive = ConversationManager.Instance.DialoguePanel.gameObject.activeInHierarchy;
        if (isDialogueActive)
        {
            Debug.LogWarning("Dialogue panel is active. Disabling components.");
            ToggleComponents(false);
        }
        else
        {
            Debug.LogWarning("Dialogue panel is not active. Enabling components.");
            ToggleComponents(true);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene {scene.name} loaded. Updating component references.");
        PopulateComponentReferences();
    }

    private void PopulateComponentReferences()
    {
        componentReferences.Clear();
        foreach (string componentName in componentsToDisable)
        {
            GameObject foundObject = FindGameObjectByName(componentName);
            if (foundObject != null)
            {
                componentReferences[componentName] = foundObject;
                Debug.Log($"Found {componentName} and added to references.");
            }
            else
            {
                Debug.LogWarning($"Failed to find {componentName} during initialization.");
            }
        }
    }

    private void ToggleComponents(bool enable)
    {
        foreach (var componentName in componentsToDisable)
        {
            if (componentReferences.TryGetValue(componentName, out GameObject component))
            {
                component.SetActive(enable);
                Debug.Log($"Set {componentName} active state to {enable}");
            }
            else
            {
                Debug.LogWarning($"Failed to find {componentName} in component references.");
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

    void OnDestroy()
    {
        if (Instance == this)
        {
            // Unregister from the sceneLoaded event when the object is destroyed
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
