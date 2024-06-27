using UnityEngine;

public class DestroyNPCObject : MonoBehaviour
{
    // Method to destroy the object with the specified name, optionally searching within children of a parent object
    private void DestroyNPC(string objectName, GameObject parent = null)
    {
        GameObject objectToDestroy = null;

        if (parent != null)
        {
            // Search for the object within the children of the specified parent
            Transform childTransform = parent.transform.Find(objectName);
            if (childTransform != null)
            {
                objectToDestroy = childTransform.gameObject;
            }
        }
        else
        {
            // Search for the object in the entire scene
            objectToDestroy = GameObject.Find(objectName);
        }

        // Destroy the object if found
        if (objectToDestroy != null)
        {
            Destroy(objectToDestroy);
            Debug.Log($"Object '{objectName}' destroyed.");
        }
        else
        {
            Debug.LogWarning($"Object '{objectName}' not found.");
        }
    }

    public void ForDestroyNPC(string objectName)
    {
        DestroyNPC(objectName);
    }
}




