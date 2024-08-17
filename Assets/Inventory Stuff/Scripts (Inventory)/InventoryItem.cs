using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI stackCountText;

    [HideInInspector] public Item item;
    [HideInInspector] public int stackCount = 1;

    // Interaction panel variables
    public GameObject interactPanel_1;
    public GameObject interactPanel_2;
    public bool isInteractOpen = false;

    private GameObject currentUsableUIPanel;

    public void InitialiseItem(Item newItem)
    {
        // Sets the inventory item's Item scriptable object to the passed in parameter
        item = newItem;
        // Sets the inventory item's sprite to the passed in Item scriptable object's icon
        image.sprite = newItem.icon;
        // Calls refresh count to update the stack count text in the UI
        RefreshCount();
    }

    public void RefreshCount()
    {
        // Sets the stack count text to the inventory item's stack count int
        stackCountText.text = stackCount.ToString();
        // Only displays the stack count text if the stack count is higher than 1
        bool shouldCountDisplay = stackCount > 1;
        stackCountText.gameObject.SetActive(shouldCountDisplay);
    }

    //-------------------------------------------------------------------------------------
    // INTERACTION PANEL
    //-------------------------------------------------------------------------------------

    public void OpenInteractionPanel()
    {
        // If there is already another interaction panel displayed, close it
        if (InventoryUI.instance.curInventoryItem != null)
        {
            InventoryUI.instance.curInventoryItem.CloseInteractionPanel();
        }
        
        if (item != null)
        {
            if (item.itemType == ItemType.Evidence || item.itemType == ItemType.Collectable)
            {
                interactPanel_1.SetActive(true);
            }
            else if (item.itemType == ItemType.Usable || item.itemType == ItemType.Exchangable)
            {
                interactPanel_2.SetActive(true);
            }

            isInteractOpen = true;
            // Set self as the current inventory item with its interaction panel displayed
            InventoryUI.instance.UpdateCurrentInventoryItem(this);
            Debug.Log("Interaction panel is open.");
        }
    }

    public void CloseInteractionPanel()
    {
        interactPanel_1.SetActive(false);
        interactPanel_2.SetActive(false);
        isInteractOpen = false;
        Debug.Log("interaction panel is closed.");
    }

    //-------------------------------------------------------------------------------------
    // EXAMINE PANEL
    //-------------------------------------------------------------------------------------

    public void CallOpenExaminePanel()
    {
        // Close the interaction panel
        if (isInteractOpen)
        {
            InventoryUI.instance.UpdateCurrentInventoryItem(null);
            CloseInteractionPanel();
        }
        // Call the InventoryUI function to open the examine panel
        InventoryUI.instance.OpenExaminePanel(item);
    }

    //-------------------------------------------------------------------------------------
    // USING ITEMS
    //-------------------------------------------------------------------------------------

    public void CallOpenUsableItemPanel()
    {
                // Close the interaction panel
                if (isInteractOpen)
                {
                    InventoryUI.instance.UpdateCurrentInventoryItem(null);
                    CloseInteractionPanel();
                }
                // Call the InventoryUI function to open the using item panel
                InventoryUI.instance.OpenUsableItemPanel(item);
            }
        
    

    public bool HasItem()
    {
        return item != null;
    }

    public Item GetItem()
    {
        if (item != null)
        {
            return item;
        }
        else
        {
            Debug.Log("Item not found.");
            return null;
        }
    }
}
