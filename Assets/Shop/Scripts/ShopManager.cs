using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using DialogueEditor;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform gridLayout;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject outlinePrefab; // Reference to the outline prefab

    [SerializeField] private Item[] shopItems; // Array of items for the shop

    private Item selectedItem;
    private GameObject currentOutline; // To hold the currently active outline instance

    [SerializeField] private bool shopOpen = false; // Flag to track if the shop panel is open

    [SerializeField] private string[] gameObjectsToDisable; // Array of canvas names to disable

    private Dictionary<string, GameObject> gameObjectReferences = new Dictionary<string, GameObject>();
    private PlayerController playerController;

    void Start()
    {
        buyButton.onClick.AddListener(OnBuyButtonClicked);
        messageText.gameObject.SetActive(false);
        AssignItemsToGridSlots();
        buyButton.gameObject.SetActive(false); // Hide the buy button initially

        // Ensure shop panel is initially closed
        if (shopPanel != null)
        {
            shopOpen = false;
            shopPanel.SetActive(false);
        }

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

    public void OpenShopPanel()
    {
        if (shopPanel != null && !shopOpen)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.DisableMovement();
            }
            // shopPanel.SetActive(true);
            StartCoroutine(OpenShopPanelDelayed());
        }
    }

    IEnumerator OpenShopPanelDelayed()
    {
        yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds

        shopPanel.SetActive(true);
        shopOpen = true;

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.DisableMovement();
        }

        // Disable interaction with other UI elements
        ToggleOtherUIInteractions(false);
    }

    public void CloseShopPanel()
    {
        if (shopPanel != null && shopOpen)
        {
            shopPanel.SetActive(false);
            shopOpen = false;

            // Enable interaction with other UI elements
            ToggleOtherUIInteractions(true);

            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.EnableMovement();
            }
        }
    }

    void ToggleOtherUIInteractions(bool enable)
    {
        foreach (string gameObjectName in gameObjectsToDisable)
        {
            if (gameObjectReferences.TryGetValue(gameObjectName, out GameObject gameObject) && gameObject != null && gameObject != shopPanel)
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


void AssignItemsToGridSlots()
    {
        // Ensure the number of items matches the number of grid slots
        if (shopItems.Length > gridLayout.childCount)
        {
            Debug.LogError("More items than grid slots!");
            return;
        }

        // Assign items to the grid slots
        for (int i = 0; i < shopItems.Length; i++)
        {
            Transform slot = gridLayout.GetChild(i);
            Image slotImage = slot.GetComponent<Image>();
            if (slotImage != null)
            {
                slotImage.sprite = shopItems[i].icon;
                slotImage.gameObject.name = shopItems[i].itemName;

                // Capture the item in a local variable
                Item capturedItem = shopItems[i];

                // Add click listener directly to the image
                slotImage.gameObject.AddComponent<Button>().onClick.AddListener(() => OnItemClicked(capturedItem));
            }
        }
    }

    void OnItemClicked(Item item)
    {
        selectedItem = item;
        buyButton.gameObject.SetActive(true);

        // Instantiate or activate outline
        SetOutlineForItem(selectedItem);
    }

    void SetOutlineForItem(Item item)
    {
        // Remove existing outline if any
        RemoveCurrentOutline();

        // Instantiate the outline prefab around the selected item
        Transform slot = FindSlotForItem(item);
        if (slot != null && outlinePrefab != null)
        {
            currentOutline = Instantiate(outlinePrefab, slot);
            currentOutline.transform.SetAsFirstSibling(); // Ensure it's rendered above the item
        }
    }

    Transform FindSlotForItem(Item item)
    {
        for (int i = 0; i < gridLayout.childCount; i++)
        {
            Transform slot = gridLayout.GetChild(i);
            if (slot.gameObject.name == item.itemName)
            {
                return slot;
            }
        }
        return null;
    }

    void RemoveCurrentOutline()
    {
        if (currentOutline != null)
        {
            Destroy(currentOutline);
        }
    }

    void OnBuyButtonClicked()
    {
        if (selectedItem != null)
        {
            if (selectedItem.itemType == ItemType.Exchangable && selectedItem.exchangedItem != null || selectedItem.itemType == ItemType.Usable)
            {
                // Attempt to exchange the item
                if (InventoryManager.instance.HasItem(selectedItem.exchangedItem) && InventoryManager.instance.ExchangeItem(selectedItem))
                {
                    Debug.Log("Item exchanged: " + selectedItem.itemName + " with " + selectedItem.exchangedItem.itemName);
                    ShowBuySuccessMessage(selectedItem.itemName);
                    InventoryUI.instance.ShowPickupDisplay(selectedItem);
                    buyButton.gameObject.SetActive(false);
                }
                else
                {
                    StartCoroutine(ShowInsufficientItemsMessage("Not enough items to exchange!"));
                }
            }
            else
            {
                Debug.LogWarning("Selected item is not exchangeable or has no exchanged item set.");
            }
        }
        else
        {
            Debug.LogWarning("No item selected.");
        }
    }

    void ShowBuySuccessMessage(string itemName)
    {
        Color color = HexToColor("#EA9823");

        messageText.text = "You bought " + itemName;
        messageText.color = color; // Set the color
        messageText.gameObject.SetActive(true);
        StartCoroutine(HideMessageAfterDelay());
    }

    Color HexToColor(string hex)
    {
        Color color = Color.white;
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }

    IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        messageText.gameObject.SetActive(false);
    }

    IEnumerator ShowInsufficientItemsMessage(string message)
    {
        Color color = HexToColor("#CC2D2D");

        messageText.text = message;
        messageText.color = color; // Set the color
        messageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        messageText.gameObject.SetActive(false);
    }
}

