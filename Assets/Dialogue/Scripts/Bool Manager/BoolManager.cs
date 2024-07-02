using UnityEngine;

public class BoolManager : MonoBehaviour
{
    // Singleton instance
    public static BoolManager Instance { get; private set; }

    // Boolean variables
    public bool SheriffIanDay1Town1;
    public bool EdithDay1Street;
    public bool EdithDay1StreetFindRaymond;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Ensures only one instance exists
        }
    }

    // Method to set a boolean variable based on its name
    public void SetBool(string boolName, bool value)
    {
        switch (boolName)
        {
            case "SheriffIanDay1Town1":
                SheriffIanDay1Town1 = value;
                break;
            case "EdithDay1Street":
                EdithDay1Street = value;
                break;
            case "EdithDay1StreetFindRaymond":
                EdithDay1StreetFindRaymond = value;
                break;
            // Add cases for additional boolean variables
            default:
                Debug.LogWarning($"Bool '{boolName}' not found.");
                break;
        }
    }

    // Method to get the value of a boolean variable based on its name
    public bool GetBool(string boolName)
    {
        switch (boolName)
        {
            case "SheriffIanDay1Town1":
                return SheriffIanDay1Town1;
            case "EdithDay1Street":
                return EdithDay1Street;
            case "EdithDay1StreetFindRaymond":
                return EdithDay1StreetFindRaymond;
            // Add cases for additional boolean variables
            default:
                Debug.LogWarning($"Bool '{boolName}' not found.");
                return false; // Or handle default case as needed
        }
    }
}

