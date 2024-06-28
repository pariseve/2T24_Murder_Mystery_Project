using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance; // Singleton instance

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object alive across scenes if needed
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    public void MarkObjectDestroyed(string objectName)
    {
        PlayerPrefs.SetInt(objectName, 1); // 1 means destroyed
        PlayerPrefs.Save();
    }

    public bool IsObjectDestroyed(string objectName)
    {
        return PlayerPrefs.GetInt(objectName, 0) == 1; // 1 means destroyed, 0 means not destroyed
    }
}

