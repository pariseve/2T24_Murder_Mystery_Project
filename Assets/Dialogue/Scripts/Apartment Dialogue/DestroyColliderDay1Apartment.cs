using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyColliderDay1Apartment : MonoBehaviour
{
    [SerializeField] private GameObject itemToMonitor;
    [SerializeField] private GameObject[] objectsToDelete;

    void Update()
    {
        // Check if the item to monitor is null (destroyed)
        if (itemToMonitor == null)
        {
            // If the item is destroyed, delete all objects in the array
            foreach (GameObject obj in objectsToDelete)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }

            Destroy(this.gameObject);
        }
    }
}
