using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;

    public Transform itemsParent;
    public GameObject inventoryUI;
    public GameObject examinePanel;
    public TextMeshProUGUI examineNameText;
    public TextMeshProUGUI examineDescriptionText;
    public Image examineImage;

    public TextMeshProUGUI pickupText;

    private bool isOpen = false;
    private Inventory inventory;
    private InventorySlot[] slots;
    private InventorySlot currentSlot;

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
        inventory = Inventory.instance;
        // Connects UpdateUI function to the onItemChangedCallback event (singal) in ItemPickup
        inventory.onItemChangedCallback += UpdateUI;
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

        if (currentSlot != null && currentSlot.isInteractOpen)
        {
            currentSlot.CloseInteractionPanel();
        }
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
                Debug.Log("Cleared slot from InventoryUI.");
                slots[i].ClearSlot();
            }
        }
    }

    public void ShowPickupText(string text)
    {
        if (pickupText != null)
        {
            pickupText.text = text;
            pickupText.alpha = 1f;
            LeanTween.alphaText(pickupText.rectTransform, 1f, 1f).setEase(LeanTweenType.easeInCirc).setOnComplete(() =>
            {
                Invoke("ClearPickupText", 2f);
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
        LeanTween.scale(examinePanel, new Vector2(0, 0), 1f).setEase(LeanTweenType.easeInBack).setOnComplete(() =>
        {
            examinePanel.SetActive(false);
        });
    }

    public void SetCurrentSlot(InventorySlot slot)
    {
        currentSlot = slot;
    }
}
