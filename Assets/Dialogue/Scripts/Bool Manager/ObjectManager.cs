using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectKeyPair
{
    public string objectName;
    public string objectKey;
}

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance; // Singleton instance

    public List<ObjectKeyPair> objectKeyPairs; // List of key-value pairs

    private Dictionary<string, string> objectKeys; // Dictionary for runtime use

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object alive across scenes if needed
            InitializeObjectKeys();
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    public void InitializeObjectKeys()
    {
        objectKeys = new Dictionary<string, string>();

        foreach (ObjectKeyPair pair in objectKeyPairs)
        {
            if (!objectKeys.ContainsKey(pair.objectName))
            {
                objectKeys.Add(pair.objectName, pair.objectKey);
                // Initialize the PlayerPrefs key to "active"
                if (!PlayerPrefs.HasKey(pair.objectKey))
                {
                    PlayerPrefs.SetString(pair.objectKey, "active");
                }
            }
        }

        // Save PlayerPrefs changes
        PlayerPrefs.Save();

        // Debug.Log or check objectKeys here
        foreach (var pair in objectKeys)
        {
            Debug.Log($"ObjectKeyPair: {pair.Key} -> {pair.Value} initialized to {PlayerPrefs.GetString(pair.Value)}");
        }
    }

    public void MarkObjectDestroyed(string objectName)
    {
        if (objectKeys.TryGetValue(objectName, out string key))
        {
            PlayerPrefs.SetString(key, "destroyed"); // Mark as destroyed
            PlayerPrefs.Save();
            Debug.Log($"Marked {objectName} as destroyed with key {key}.");
        }
        else
        {
            Debug.LogWarning($"Object name {objectName} not found in objectKeys.");
        }
    }

    public bool IsObjectDestroyed(string objectName)
    {
        if (objectKeys.TryGetValue(objectName, out string key))
        {
            // Check if the PlayerPrefs key corresponding to objectKey is set to "destroyed"
            bool isDestroyed = PlayerPrefs.GetString(key, "active") == "destroyed";
            Debug.Log($"IsObjectDestroyed check for {objectName} with key {key}: {isDestroyed}");
            return isDestroyed;
        }
        else
        {
            Debug.LogWarning($"Object name {objectName} not found in objectKeys.");
            return false;
        }
    }

public void DebugCheckForKey(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string value = PlayerPrefs.GetString(key);
            if (value == "destroyed")
            {
                Debug.Log($"PlayerPrefs key '{key}' exists and has value 'destroyed'.");
            }
            else
            {
                Debug.Log($"PlayerPrefs key '{key}' exists but does not have value 'destroyed'. Current value: {value}");
            }
        }
        else
        {
            Debug.Log($"PlayerPrefs key '{key}' does not exist.");
        }
    }
}