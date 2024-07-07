using UnityEngine;

public class ShowObject : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivate;
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

        // Activate the object if all booleans are true and it's not already active
        if (allBoolsTrue && objectToActivate != null && !objectToActivate.activeSelf)
        {
            objectToActivate.SetActive(true);
            Debug.Log($"Activated {objectToActivate.name}");
        }
        // Optional: Deactivate the object if any boolean is false and it's currently active
        else if (!allBoolsTrue && objectToActivate != null && objectToActivate.activeSelf)
        {
            objectToActivate.SetActive(false);
            Debug.Log($"Deactivated {objectToActivate.name}");
        }
        else
        {
            Debug.Log("Bool not found");
        }
    }
}