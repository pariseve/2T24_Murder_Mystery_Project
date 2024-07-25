using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DragUIPuzzleManager : MonoBehaviour
{
    public static DragUIPuzzleManager Instance;

    [SerializeField] private DraggableItem[] draggableItems; // Assign all draggable items
    [SerializeField] private TextMeshProUGUI completeText; // Assign the TextMeshProUGUI component
    [SerializeField] private Item[] itemsToRemove; // Items to remove from the inventory
    [SerializeField] private Item itemToAdd; // Item to add to the inventory
    [SerializeField] private float proximityThreshold = 50f; // Set an appropriate proximity threshold
    [SerializeField] private GameObject parentObject;
    [SerializeField] private string boolName;

    private bool isPuzzleComplete = false; // Flag to check if the puzzle is complete

    [SerializeField] private Item[] requiredInventoryItems;

    [SerializeField] private string[] gameObjectsToDisable; // Array of canvas names to disable
    private Dictionary<string, GameObject> gameObjectReferences = new Dictionary<string, GameObject>();

    private void Start()
    {
        completeText.gameObject.SetActive(false);
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

    private void Update()
    {
        foreach (var item in requiredInventoryItems)
        {
            if (InventoryManager.instance.HasItem(item))
            {
                ToggleOtherUIInteractions(false);
                parentObject.gameObject.SetActive(true);
                CheckAndUpdatePlayerMovement();
                if (!isPuzzleComplete)
                {
                    CheckPuzzleCompletion();
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

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void CheckPuzzleCompletion()
    {
        bool allCorrect = true;

        foreach (var item in draggableItems)
        {
            if (!IsItemInProximityOfOthers(item))
            {
            allCorrect = false;
            }
        }

        if (allCorrect && !isPuzzleComplete)
        {
            isPuzzleComplete = true; // Set the flag to true to ensure actions are performed only once
            StartCoroutine(ShowCompleteText());
            SetBoolVariable();
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

    private bool IsItemInProximityOfOthers(DraggableItem draggableItem)
    {
        // Get the target RectTransform of the draggable item
        RectTransform draggableTargetRectTransform = draggableItem.GetTargetRectTransform();
        if (draggableTargetRectTransform == null)
        {
            Debug.LogWarning($"Target RectTransform for {draggableItem.name} is not assigned.");
            return false;
        }

        // Convert draggable item's target anchoredPosition to world position
        Vector3 draggableWorldPos = draggableTargetRectTransform.TransformPoint(draggableTargetRectTransform.rect.center);

        bool isInProximity = false;

        // Debugging information for the specified draggable item
        Debug.Log($"Checking proximity for {draggableItem.name} at position {draggableWorldPos}");

        // Debug information for all draggable items
        Debug.Log("Draggable Items and their positions:");
        foreach (var item in draggableItems)
        {
            RectTransform itemTargetRectTransform = item.GetTargetRectTransform();
            if (itemTargetRectTransform == null)
            {
                Debug.LogWarning($"Target RectTransform for {item.name} is not assigned.");
                continue;
            }

            // Convert target item's anchoredPosition to world position
            Vector3 itemWorldPos = itemTargetRectTransform.TransformPoint(itemTargetRectTransform.rect.center);
            Debug.Log($"{item.name} at position {itemWorldPos}");
        }

        // Check proximity for the specified draggable item
        foreach (var item in draggableItems)
        {
            if (draggableItem == item) continue; // Skip checking against itself

            RectTransform targetRectTransform = item.GetTargetRectTransform();
            if (targetRectTransform == null)
            {
                Debug.LogWarning($"Target RectTransform for {item.name} is not assigned.");
                continue;
            }

            // Convert targetRectTransform's anchoredPosition to world position
            Vector3 targetWorldPos = targetRectTransform.TransformPoint(targetRectTransform.rect.center);
            float distance = Vector3.Distance(draggableWorldPos, targetWorldPos);

            // Log positions and distance for debugging
            Debug.Log($"Comparing {draggableItem.name} at {draggableWorldPos} with {item.name} at {targetWorldPos}");
            Debug.Log($"Distance: {distance}");

            if (distance <= proximityThreshold)
            {
                isInProximity = true;
                Debug.Log($"{draggableItem.name} is within proximity of {item.name}");
                // No need to break; because we want to check all other items
            }
        }

        // Check if it is in proximity with at least one other item
        Debug.Log($"{draggableItem.name} is in proximity of another item: {isInProximity}");

        return isInProximity;
    }

    private IEnumerator ShowCompleteText()
    {
        completeText.gameObject.SetActive(true);
        completeText.transform.localScale = Vector3.zero;
        LeanTween.scale(completeText.gameObject, Vector3.one, 0.5f).setEaseOutBounce();

        yield return new WaitForSeconds(2f);

        LeanTween.scale(completeText.gameObject, Vector3.zero, 0.5f).setEaseInBounce().setOnComplete(() =>
        {
            completeText.gameObject.SetActive(false);
        });

        // Manage inventory
        foreach (var item in itemsToRemove)
        {
            InventoryManager.instance.RemoveItem(item);
        }
        InventoryManager.instance.AddItem(itemToAdd);

        // Optionally destroy the UI
        foreach (var item in draggableItems)
        {
            Destroy(item.gameObject);
        }
    }

    public void CloseUI()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.EnableMovement();
        }
        Destroy(gameObject); // Destroy the GameObject that this script is attached to
        ToggleOtherUIInteractions(true);
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
        foreach (var draggableItem in draggableItems)
        {
            if (draggableItem.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
}



