using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image icon; // Reference to the icon image for the item
    private Item item; // Reference to the item associated with this slot

    public TextMeshProUGUI amountText;

    public GameObject interactEvidencePanel;
    public GameObject interactUsablePanel;
    public bool isInteractOpen = false;

    public GameObject examinePanel;
    public TextMeshProUGUI examineNameText;
    public TextMeshProUGUI examineDescriptionText;
    public Image examineImage;

    private void Awake()
    {
        interactUsablePanel.SetActive(false);
        interactEvidencePanel.SetActive(false);
        if (amountText != null) amountText.gameObject.SetActive(false);
    }

    public void AddItem(Item newItem)
    {
        if (newItem == null)
            return;

        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        Debug.Log("Item added to slot: " + item.itemName + ", Sprite: " + item.icon.name);

        if (item.isStackable)
        {
            UpdateAmount();
        }
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null; // Clear the sprite
        icon.enabled = false; // Hide the icon
        if (amountText != null)
        {
            amountText.gameObject.SetActive(false);
            amountText.text = ""; // Clear the amount text
        }
        Debug.Log("Slot cleared");
    }

    public void UpdateAmount()
    {
        if (amountText != null && item != null && item.isStackable)
        {
            amountText.text = item.amount.ToString();
            amountText.gameObject.SetActive(true);
        }
    }


    public void OpenInteractionPanel()
    {
        if (item != null)
        {
            InventoryUI.instance.SetCurrentSlot(this); // Set the current slot in InventoryUI
            if (item.itemType == ItemType.Usable && interactUsablePanel != null)
            {
                interactUsablePanel.SetActive(true);
            }
            else if (item.itemType == ItemType.Evidence && interactEvidencePanel != null)
            {
                interactEvidencePanel.SetActive(true);
            }
            isInteractOpen = true;
            Debug.Log("Interaction panel is open");
        }
    }

    public void CloseInteractionPanel()
    {
        if (interactUsablePanel != null) interactUsablePanel.SetActive(false);
        if (interactEvidencePanel != null) interactEvidencePanel.SetActive(false);
        isInteractOpen = false;
        Debug.Log("Interaction panel is closed");
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

            InventoryUI.instance.SetCurrentSlot(this); // Set the current slot in InventoryUI
            Debug.Log("Set current slot in InventoryUI");

            if (isInteractOpen)
            {
                CloseInteractionPanel();
            }
        }
    }
}
