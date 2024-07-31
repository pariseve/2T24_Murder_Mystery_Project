using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoreEvidenceItems : MonoBehaviour
{
    public GameObject[] evidenceSlots;
    public string targetSceneName;

    private bool hasStoredItems = false;

    public LayerMask interactableLayerMask;

    //for opening examine panel
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 1.0f); // Visualize the raycast

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableLayerMask))
            {
                EvidenceSlot evidenceSlot = hit.collider.GetComponent<EvidenceSlot>();
                if (evidenceSlot != null)
                {
                    evidenceSlot.Interact();
                }
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    //when the target scene is loaded, trigger store items function
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
            StoreItems();
           
    }
    //searches the inventory for all items under evidence type, removes them from inventory and adds them to evidence slot
    public void StoreItems()
    {
        List<Item> evidenceItems = InventoryManager.instance.FindAllItemsOfType(ItemType.Evidence);

        int slotIndex = 0;

        foreach (Item evidenceItem in evidenceItems)
        {
            while (slotIndex < evidenceSlots.Length)
            {
                GameObject slot = evidenceSlots[slotIndex];
                EvidenceSlot evidenceSlot = slot.GetComponent<EvidenceSlot>();

                if (evidenceSlot != null && !evidenceSlot.HasItem())
                {
                    bool removed = InventoryManager.instance.RemoveItem(evidenceItem);

                    if (removed)
                    {
                        // Set the slotID of the item before storing it
                        evidenceItem.slotID = evidenceSlot.SlotID;
                        evidenceSlot.StoreItem(evidenceItem);
                        Debug.Log($"Stored {evidenceItem.itemName} in slot {slotIndex}");
                        break;
                    }
                }

                slotIndex++;
            }

            if (slotIndex >= evidenceSlots.Length)
            {
                Debug.Log("Not enough slots to store all evidence items.");
                break;
            }
        }
    }

}
