using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image icon; // Reference to the icon image for the item
    private Item item; // Reference to the item associated with this slot
    private int itemAmount = 0;
    public TextMeshProUGUI amountText;

    public GameObject interactionsPanel;
    public bool isInteractOpen = false;

    public GameObject examinePanel;
    public TextMeshProUGUI examineNameText;
    public TextMeshProUGUI examineDescriptionText;
    public Image examineImage;

    public void AddItem(Item newItem)
    {
        Debug.Log("Adding item: " + newItem.itemName);

        if (item != null && item.itemName == newItem.itemName && item.isStackable)
        {
            // Item already exists in slot and is stackable, increase amount
            itemAmount += 1;
            amountText.text = itemAmount.ToString();
            amountText.gameObject.SetActive(true);
            Debug.Log("Stacking item: " + item.itemName + ", Amount: " + itemAmount);
        }
        else if (item == null)
        {
            Debug.Log("Added new item: " + newItem.itemName);
            // Slot is empty, add the new item
            item = newItem;
            icon.sprite = item.icon;
            icon.enabled = true;
            itemAmount = 1;
            amountText.text = itemAmount.ToString();
            amountText.gameObject.SetActive(false);
        }
        else
        {
            // Slot is occupied by a different item or the item is not stackable, do nothing
            Debug.LogWarning("Cannot add item to slot. Slot is occupied by a different item or item is not stackable.");
        }

        Debug.Log("Current item in slot: " + (item == null ? "None" : item.itemName));
    }

    public void ClearSlot()
    {
        item = null;
        itemAmount = 0;
        icon.sprite = null; // Clear the sprite
        icon.enabled = false; // Hide the icon
        amountText.gameObject.SetActive(false); // Hide the amount text
        Debug.Log("Slot cleared"); // Debug log for confirmation
    }

    public void OpenInteractionPanel()
    {
        if (item != null)
        {
            interactionsPanel.SetActive(true);
            isInteractOpen = true;
            Debug.Log("Interaction panel is open");
        }
    }

    public void CloseInteractionPanel()
    {
        interactionsPanel.SetActive(false);
        isInteractOpen = false;
        Debug.Log("interaction panel is closed");
    }

    public void OpenExaminePanel()
    {
        if (item != null)
        {
            // Update UI elements in the examine panel
            examinePanel.SetActive(true);
            LeanTween.scale(examinePanel, new Vector2(1, 1), 1f).setEase(LeanTweenType.easeOutBack);
            examineNameText.text = item.itemName;
            examineDescriptionText.text = item.description;
            examineImage.sprite = item.icon;

            if (isInteractOpen)
            {
                CloseInteractionPanel();
            }
        }
    }

    public bool HasItem()
    {
        return item != null;
    }

    public Item GetItem()
    {
        return item;
    }
}
