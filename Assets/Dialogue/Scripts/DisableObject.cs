using UnityEngine;

public class DisableObject : MonoBehaviour
{
    [SerializeField] private GameObject objectToToggle;
    [SerializeField] private string[] boolNames; // Array of boolean names

    void Update()
    {
        // Check if all the specified booleans are true
        bool allBoolsTrue = true;
        foreach (string boolName in boolNames)
        {
            if (!BoolManager.Instance.GetBool(boolName))
            {
                allBoolsTrue = false;
                break;
            }
        }

        // Deactivate the object if all booleans are true and it's currently active
        if (allBoolsTrue && objectToToggle != null && objectToToggle.activeSelf)
        {
            objectToToggle.SetActive(false);
            Debug.Log($"Deactivated {objectToToggle.name}");
        }
        // Activate the object if any boolean is false and it's not already active
        else if (!allBoolsTrue && objectToToggle != null && !objectToToggle.activeSelf)
        {
            objectToToggle.SetActive(true);
            Debug.Log($"Activated {objectToToggle.name}");
        }
    }
}

