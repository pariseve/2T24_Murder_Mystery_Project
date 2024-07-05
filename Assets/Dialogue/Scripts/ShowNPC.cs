using UnityEngine;

public class ShowNPC : MonoBehaviour
{
    [SerializeField] private GameObject npcToActivate;
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
        if (allBoolsTrue && npcToActivate != null && !npcToActivate.activeSelf)
        {
            npcToActivate.SetActive(true);
            Debug.Log($"Activated {npcToActivate.name}");
        }
        // Optional: Deactivate the object if any boolean is false and it's currently active
        else if (!allBoolsTrue && npcToActivate != null && npcToActivate.activeSelf)
        {
            npcToActivate.SetActive(false);
            Debug.Log($"Deactivated {npcToActivate.name}");
        }
        else
        {
            Debug.Log("Bool not found");
        }
    }
}