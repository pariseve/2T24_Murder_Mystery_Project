using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image icon; // Reference to the icon image for the item
    private Item item; // Reference to the item associated with this slot

    public GameObject interactionsPanel;
    public bool isInteractOpen = false;

    public GameObject examinePanel;
    public TextMeshProUGUI examineNameText;
    public TextMeshProUGUI examineDescriptionText;
    public Image examineImage;

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        Debug.Log("Item added to slot: " + item.itemName + ", Sprite: " + item.icon.name);
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null; // Clear the sprite
        icon.enabled = false; // Hide the icon
        Debug.Log("Slot cleared"); // Debug log for confirmation
    }

    public void OpenInteractionPanel()
    {
        if(item != null)
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

}
