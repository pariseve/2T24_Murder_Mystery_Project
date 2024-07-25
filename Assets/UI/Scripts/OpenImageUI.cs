using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenImageUI : MonoBehaviour
{
    [SerializeField] private GameObject parentObject; // The UI panel to be shown or hidden
    [SerializeField] private string[] gameObjectsToDisable;
    private Dictionary<string, GameObject> gameObjectReferences = new Dictionary<string, GameObject>();

    [SerializeField] private Item[] requiredInventoryItems;

    private void Start()
    {
        parentObject.gameObject.SetActive(false);

        // Populate the dictionary with references to the objects to disable/enable
        foreach (string gameObjectName in gameObjectsToDisable)
        {
            GameObject foundObject = FindGameObjectByName(gameObjectName);
            if (foundObject != null)
            {
                gameObjectReferences[gameObjectName] = foundObject;
            }
            else
            {
                Debug.LogWarning($"Failed to find {gameObjectName} during initialization.");
            }
        }
    }

    private void Update()
    {
        foreach (var item in requiredInventoryItems)
        {
            if (InventoryManager.instance.HasItem(item))
            {
                if (!parentObject.gameObject.activeSelf)
                {
                    ToggleOtherUIInteractions(true);
                    StartCoroutine(AnimateUIOpen());
                }
            }
                else
                {
                    ToggleOtherUIInteractions(true);
                    PlayerController playerController = FindObjectOfType<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.EnableMovement();
                    }
                    Destroy(gameObject);
                }
            }
        }

    private IEnumerator AnimateUIOpen()
    {
        // Disable UI components initially
        parentObject.gameObject.SetActive(true);
        // Ensure player movement is disabled
        CheckAndUpdatePlayerMovement();
        ToggleOtherUIInteractions(false);

        // Initialize the panel's scale to zero for animation
        parentObject.transform.localScale = Vector3.zero;

        // Animate the panel's scale with bounce effect
        LeanTween.scale(parentObject, Vector3.one, 0.5f).setEaseOutBounce();

        // Wait for the animation to complete
        yield return new WaitForSeconds(0.5f); // Duration of the bounce animation
    }

    private IEnumerator AnimateUIClose()
    {
        // Animate the panel shrinking with bounce effect
        LeanTween.scale(parentObject, Vector3.zero, 0.5f).setEaseInBounce().setOnComplete(() =>
        {
            // Enable other UI interactions
            // ToggleOtherUIInteractions(true);

            // Ensure player movement is enabled
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.EnableMovement();
            }
        });

        // Wait for the animation to complete
        yield return new WaitForSeconds(0.5f); // Duration of the bounce animation

        // Destroy the GameObject only after the animation and other actions are completed
        Destroy(gameObject);
    }

    GameObject FindGameObjectByName(string gameObjectName)
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

    void ToggleOtherUIInteractions(bool enable)
    {
        foreach (string gameObjectName in gameObjectsToDisable)
        {
            if (gameObjectReferences.TryGetValue(gameObjectName, out GameObject gameObject) && gameObject != null && gameObject != parentObject)
            {
                gameObject.SetActive(enable);
                Debug.Log($"Set {gameObjectName} active state to {enable}");
            }
            else
            {
                Debug.LogWarning($"Failed to find or activate {gameObjectName}");
            }
        }
    }

    public bool IsUIActive()
    {
            if (parentObject.gameObject.activeSelf)
            {
                return true;
            }
        return false;
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

    public void CloseUI()
    {
        if (parentObject.gameObject.activeSelf)
        {
            StartCoroutine(AnimateUIClose());
            ToggleOtherUIInteractions(true);
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.EnableMovement();
            }
        }
        else
        {
            ToggleOtherUIInteractions(true);
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.EnableMovement();
            }
            Destroy(gameObject); // Destroy the GameObject that this script is attached to
        }
    }
}