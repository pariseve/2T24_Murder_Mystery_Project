using UnityEngine;
using System.Collections;
using System.Linq;

public class ObjectResetManager : MonoBehaviour
{
    [SerializeField] private string[] objectNamesToDestroy; // Names of objects to be destroyed
    [SerializeField] private GameObject[] prefabsToInstantiate; // Array of prefabs to be instantiated

    public void NewGame()
    {
        StartCoroutine(ManageObjects());
    }

    private IEnumerator ManageObjects()
    {
        // Destroy objects by name in the current scene
        DestroyObjectsInScene();

        // Destroy objects by name in DontDestroyOnLoad
        DestroyObjectsInDontDestroyOnLoad();

        // Wait for 0.5 seconds
        yield return new WaitForSeconds(0.1f);

        // Instantiate prefabs
        InstantiatePrefabs();
    }

    private void DestroyObjectsInScene()
    {
        foreach (string objectName in objectNamesToDestroy)
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj != null)
            {
                Destroy(obj);
            }
            else
            {
                Debug.LogWarning("Object with name " + objectName + " not found in the current scene.");
            }
        }
    }

    private void DestroyObjectsInDontDestroyOnLoad()
    {
        // Get all objects in DontDestroyOnLoad scene
        var dontDestroyOnLoadObjects = FindObjectsOfType<GameObject>(true)
            .Where(go => go.scene.name == "DontDestroyOnLoad")
            .ToArray();

        foreach (string objectName in objectNamesToDestroy)
        {
            GameObject obj = dontDestroyOnLoadObjects.FirstOrDefault(o => o.name == objectName);
            if (obj != null)
            {
                Destroy(obj);
            }
            else
            {
                Debug.LogWarning("Object with name " + objectName + " not found in DontDestroyOnLoad.");
            }
        }
    }

    private void InstantiatePrefabs()
    {
        foreach (GameObject prefab in prefabsToInstantiate)
        {
            if (prefab != null)
            {
                Instantiate(prefab); // Instantiates at (0, 0, 0) by default
            }
            else
            {
                Debug.LogWarning("Prefab is null.");
            }
        }
    }
}



