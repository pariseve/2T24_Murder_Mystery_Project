using UnityEngine;

public class DestroyColliderSave : MonoBehaviour
{
    [SerializeField] private string objectNameToMonitor;

    void Start()
    {
        // Check if this object should be destroyed based on saved state
        if (ObjectManager.Instance.IsObjectDestroyed(objectNameToMonitor))
        {
            Destroy(gameObject);
        }
    }

    // Method to handle destruction via button click
    public void DestroyObject()
    {
        ObjectManager.Instance.MarkObjectDestroyed(objectNameToMonitor);
        Destroy(gameObject);
    }
}

