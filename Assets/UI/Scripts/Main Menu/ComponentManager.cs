using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ComponentManager : MonoBehaviour
{
    [SerializeField] private string[] componentsToDisable; // Names of components to disable/enable
    [SerializeField] private string[] sceneNamesToDisableComponents; // Scene names where components should be disabled

    private Dictionary<string, GameObject> componentReferences = new Dictionary<string, GameObject>();

    public static ComponentManager Instance { get; private set; }

    private void Awake()
    {
        /*
        // Subscribe to scene change events
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Populate the dictionary initially
        PopulateComponentReferences();

        // Debug: Check initial state
        CheckAndToggleComponents(SceneManager.GetActiveScene().name);
    */}

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // Subscribe to scene change events
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Populate the dictionary initially
        PopulateComponentReferences();

        // Debug: Check initial state
        CheckAndToggleComponents(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        // Unsubscribe from scene change events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");
        PopulateComponentReferences();
        CheckAndToggleComponents(scene.name);
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

    private void CheckAndToggleComponents(string sceneName)
    {
        bool shouldDisableComponents = ShouldDisableComponentsInScene(sceneName);
        Debug.Log($"Should disable components: {shouldDisableComponents}");
        ToggleComponents(!shouldDisableComponents);
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

    private bool ShouldDisableComponentsInScene(string sceneName)
    {
        foreach (string disableScene in sceneNamesToDisableComponents)
        {
            if (disableScene == sceneName)
            {
                return true;
            }
        }
        return false;
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


