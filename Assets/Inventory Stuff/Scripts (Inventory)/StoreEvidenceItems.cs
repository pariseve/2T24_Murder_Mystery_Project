using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoreEvidenceItems : MonoBehaviour
{
    public GameObject[] evidenceSlots;
    public string targetSceneName;

    public LayerMask interactableLayerMask;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 1.0f); // Visualize the raycast

            // Perform raycast with layer mask
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableLayerMask))
            {
                Debug.Log("Raycast hit: " + hit.collider.name);

                EvidenceSlot evidenceSlot = hit.collider.GetComponent<EvidenceSlot>();
                if (evidenceSlot != null)
                {
                    Debug.Log("EvidenceSlot found: " + evidenceSlot.name);
                    evidenceSlot.Interact();
                }
                else
                {
                    Debug.Log("Hit object is not an EvidenceSlot.");
                }
            }
            else
            {
                Debug.Log("Raycast did not hit anything.");
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == targetSceneName)
        {
            StoreItems();
        }
    }

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
                        evidenceSlot.StoreItem(evidenceItem);
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
