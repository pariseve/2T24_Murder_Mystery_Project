using UnityEngine;

public class DestroyColliderDay1Apartment : MonoBehaviour
{
    [SerializeField] private string objectNameToMonitor;

    void Start()
    {
        // Check if this object should be destroyed based on saved state
        if (PlayerPrefs.GetInt(objectNameToMonitor, 0) == 1) // 1 means destroyed, 0 means not destroyed
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // Mark object as destroyed when it is destroyed
        PlayerPrefs.SetInt(objectNameToMonitor, 1);
        PlayerPrefs.Save();
    }

    // Method to handle destruction via button click
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}

