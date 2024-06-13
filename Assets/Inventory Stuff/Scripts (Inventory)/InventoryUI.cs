using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance; // Singleton instance

    public Transform itemsParent;
    public GameObject inventoryUI;
    public GameObject examinePanel;
    public TextMeshProUGUI examineNameText;
    public TextMeshProUGUI examineDescriptionText;
    public Image examineImage;

    public TextMeshProUGUI pickupText;

    private bool isOpen = false;

    Inventory inventory;
    InventorySlot[] slots;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Get the singleton instance of the inventory
        inventory = Inventory.instance;
        // Subscribe UpdateUI to the onItemChangedCallback
        inventory.onItemChangedCallback += UpdateUI;

        // Get all the InventorySlot components in the children of itemsParent
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

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
        LeanTween.moveLocalY(inventoryUI, 440f, 0.5f).setEase(LeanTweenType.easeOutQuart);
        isOpen = true;
    }

    void CloseUI()
    {
        LeanTween.moveLocalY(inventoryUI, 650f, 0.5f).setEase(LeanTweenType.easeInQuart);
        isOpen = false;
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

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
    }

    public void CloseExaminePanel()
    {
        LeanTween.scale(examinePanel, new Vector2(0, 0), 1f)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() =>
            {
                examinePanel.SetActive(false); // This will be called after the animation completes
            });
    }
}
