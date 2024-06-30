using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance; // Singleton instance

    public GameObject display;
    public Transform grid;
    private bool isOpen = false;

    // Interaction panel variables
    public InventoryItem curInventoryItem;

    // Examine panel variables
    public GameObject examinePanel;
    public TextMeshProUGUI examineNameText;
    public TextMeshProUGUI examineDescriptionText;
    public Image examineImage;

    // Pickup text variables
    public TextMeshProUGUI pickupText;


    //InventorySlot[] slots;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    //void Start()
    //{
    //    // Subscribe UpdateUI to the onItemChangedCallback
    //    Inventory.instance.onItemChangedCallback += UpdateUI;

    //    // Get all the InventorySlot components in the children of itemsParent
    //    slots = grid.GetComponentsInChildren<InventorySlot>();
    //}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !isOpen)
        {
            OpenUI();
        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            CloseUI();
        }
    }

    void OpenUI()
    {
        LeanTween.moveLocalY(display, 440f, 0.5f).setEase(LeanTweenType.easeOutQuart);
        isOpen = true;
    }

    void CloseUI()
    {
        LeanTween.moveLocalY(display, 650f, 0.5f).setEase(LeanTweenType.easeInQuart);

        // If there is an interaction panel open, close it
        if (curInventoryItem != null)
        {
            curInventoryItem.CloseInteractionPanel();
            curInventoryItem = null;
        }

        isOpen = false;
    }

    //void UpdateUI()
    //{
    //    for (int i = 0; i < slots.Length; i++)
    //    {
    //        if (i < Inventory.instance.items.Count)
    //        {
    //            slots[i].AddItem(Inventory.instance.items[i]);
    //        }
    //    }
    //}

    //-------------------------------------------------------------------------------------
    // PICKUP TEXT
    //-------------------------------------------------------------------------------------

    public void ShowPickupText(string text)
    {
        if (pickupText != null)
        {
            pickupText.text = text;
            pickupText.alpha = 1f; // Ensure the text is fully visible
            LeanTween.alphaText(pickupText.rectTransform, 1f, 1f).setEase(LeanTweenType.easeInCirc).setOnComplete(() =>
            {
                Invoke("ClearPickupText", 2f); // Clear the text after 2 seconds
            });
        }
        else
        {
            Debug.Log("Pickup text not found.");
        }
    }

    private void ClearPickupText()
    {
        if (pickupText != null)
        {
            LeanTween.alphaText(pickupText.rectTransform, 0f, 1.5f).setEase(LeanTweenType.easeOutCirc).setOnComplete(() =>
            {
                pickupText.text = "";
            });
        }
        else
        {
            Debug.Log("Pickup text not found.");
        }
    }

    //-------------------------------------------------------------------------------------
    // EXAMINE PANEL
    //-------------------------------------------------------------------------------------

    public void OpenExaminePanel(Item item)
    {
        if (item != null)
        {
            // Update UI elements in the examine panel
            examineNameText.text = item.itemName;
            examineDescriptionText.text = item.description;
            examineImage.sprite = item.icon;
            examinePanel.SetActive(true);
            LeanTween.scale(examinePanel, new Vector2(1, 1), 1f).setEase(LeanTweenType.easeOutBack);
        }
    }

    public void CloseExaminePanel()
    {
        LeanTween.scale(examinePanel, new Vector2(0, 0), 1f).setEase(LeanTweenType.easeInBack).setOnComplete(() =>
            {
                examinePanel.SetActive(false); // This will be called after the animation completes
            });
    }

    public void UpdateCurrentInventoryItem(InventoryItem inventoryItem)
    {
        curInventoryItem = inventoryItem;
    }
}
