using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance; // Singleton instance

    public GameObject display;
    public Transform grid;
    private bool isOpen = false;

    // Interaction panel variables
    public InventoryItem curInventoryItem;
    private GameObject currentUsableUIPanel;

    // Examine panel variables
    public GameObject examinePanel;
    public TextMeshProUGUI examineNameText;
    public TextMeshProUGUI examineDescriptionText;
    public Image examineImage;
    public RectTransform examineContentRectTransform;
    private float defaultWidth = 400f;
    private float defaultHeight = 1f;


    // Pickup Display variables
    public GameObject pickupDisplayPanel;
    public Image pickupItemImage;
    public TextMeshProUGUI pickupText;

    [SerializeField] private float animationDuration = 0.5f;


    //InventorySlot[] slots;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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

    public void ShowPickupDisplay(Item item)
    {
        if (pickupDisplayPanel != null)
        {
            pickupDisplayPanel.SetActive(true);
            pickupText.text = "[i] Item obtained: " + item.itemName;
            pickupItemImage.sprite = item.icon;
            LeanTween.scale(pickupDisplayPanel, new Vector2(1, 1), 1f).setEase(LeanTweenType.easeOutQuint);

            // Close the display panel after 3 seconds
            Invoke("EndPickupDisplay", 2f);
        }
        else
        {
            Debug.Log("Pickup text not found.");
        }
    }

    private void EndPickupDisplay()
    {
        if (pickupDisplayPanel != null)
        {
            LeanTween.scale(pickupDisplayPanel, new Vector2(0, 0), 1f).setEase(LeanTweenType.easeInQuint).setOnComplete(() =>
            {
                pickupDisplayPanel.SetActive(false);
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

    private void UpdateExaminePanel()
    {
        // Force the content size fitter to update
        LayoutRebuilder.ForceRebuildLayoutImmediate(examineContentRectTransform);

        // Optionally, scroll to the top if using a ScrollRect
        ScrollRect scrollRect = examinePanel.GetComponentInChildren<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f; // Scroll to top
        }
    }

    private void ResetExaminePanel()
    {
        // Reset size to default values
        examineContentRectTransform.sizeDelta = new Vector2(defaultWidth, defaultHeight);

        // Force a layout rebuild to apply the new size
        LayoutRebuilder.ForceRebuildLayoutImmediate(examineContentRectTransform);
    }

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

            //UpdateExaminePanel();

            // Disable player movement or any other setup needed before dialogue
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.DisableMovement();
            }
        }
    }

    public void CloseExaminePanel()
    {
        // Disable player movement or any other setup needed before dialogue
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.EnableMovement();
        }

        LeanTween.scale(examinePanel, new Vector2(0, 0), 1f).setEase(LeanTweenType.easeInBack).setOnComplete(() =>
            {
                examinePanel.SetActive(false); // This will be called after the animation completes
                ResetExaminePanel();
            });
    }

    public void UpdateCurrentInventoryItem(InventoryItem inventoryItem)
    {
        curInventoryItem = inventoryItem;
    }

    //-------------------------------------------------------------------------------------
    // USABLE ITEM UI PANEL
    //-------------------------------------------------------------------------------------

    public void OpenUsableItemPanel(Item item)
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();

        if (item != null && item.itemType == ItemType.Usable)
        {
            // Destroy the current usable UI panel if it exists
            if (currentUsableUIPanel != null)
            {
                Destroy(currentUsableUIPanel);
            }

            // Instantiate the new usable UI panel
            if (item.interactionUIPrefab != null)
            {
                currentUsableUIPanel = Instantiate(item.interactionUIPrefab);
                StartCoroutine(AnimateUI(true));
                StartCoroutine(CheckIfUsablePanelActive());

                // Disable player movement if playerController and currentUsableUIPanel are valid
                if (playerController != null && currentUsableUIPanel != null)
                {
                    playerController.DisableMovement();
                }
            }
        }
        else
        {
            // Enable movement if the item is not usable or null
            if (playerController != null)
            {
                playerController.EnableMovement();
            }
        }

        // Additional check (though this might be redundant if coroutine works as expected)
        if (currentUsableUIPanel == null || !currentUsableUIPanel.activeInHierarchy)
        {
            if (playerController != null)
            {
                playerController.EnableMovement();
            }
        }
    }

    private IEnumerator CheckIfUsablePanelActive()
    {
        yield return new WaitForSeconds(0f); // Short delay to allow for any necessary initialization

        PlayerController playerController = FindObjectOfType<PlayerController>();

        // Check if the current UI panel is inactive or destroyed and enable movement if so
        if (currentUsableUIPanel == null || !currentUsableUIPanel.activeInHierarchy)
        {
            Debug.Log("Enabled movement because usable panel is not active");

            if (playerController != null)
            {
                playerController.EnableMovement();
            }
        }
    }

    private IEnumerator AnimateUI(bool enable)
    {
        if (currentUsableUIPanel == null)
        {
            Debug.LogError("currentUsableUIPanel is null. Cannot animate.");
            yield break;
        }

        // Find the child object named "Parent Object"
        Transform parentObjectTransform = currentUsableUIPanel.transform.Find("Parent Object");
        if (parentObjectTransform == null)
        {
            Debug.LogError("Parent Object not found in currentUsableUIPanel.");
            yield break;
        }

        GameObject parentObject = parentObjectTransform.gameObject;

        if (enable)
        {
            Debug.Log("Animating Parent Object to scale up.");
            parentObject.SetActive(true);
            parentObject.transform.localScale = Vector3.zero; // Ensure starting scale is zero
            LeanTween.scale(parentObject, Vector3.one, animationDuration).setEaseOutBounce();
        }
        else
        {
            Debug.Log("Animating Parent Object to scale down.");
            LeanTween.scale(parentObject, Vector3.zero, animationDuration).setEaseInBounce().setOnComplete(() =>
            {
                parentObject.SetActive(false);
                Debug.Log("Scale down animation completed.");
            });
        }

        yield return new WaitForSeconds(animationDuration);
    }

    public void CloseUsableItemPanel()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();

        if (currentUsableUIPanel != null)
        {
            Destroy(currentUsableUIPanel);
        }

        // Enable movement if the UI panel has been destroyed or does not exist
        if (playerController != null)
        {
            playerController.EnableMovement();
        }
    }
}


