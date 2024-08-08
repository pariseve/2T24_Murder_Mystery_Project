using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TakeMedicationUI : MonoBehaviour
{
    [SerializeField] private bool needsMedication = false;
    [SerializeField] private string medicationUIName = "MedicationUI"; // The name of the instantiated UI prefab
    [SerializeField] private float animationDuration = 0.5f;

    [SerializeField] private bool isListenerSet = false;

    void Update()
    {
        GameObject medicationUI = GameObject.Find(medicationUIName);

        if (medicationUI != null && medicationUI.activeInHierarchy && !isListenerSet)
        {
            SetupButtonListener();
        }
        if (needsMedication)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.DisableMovement();
            }
        }
    }

    public IEnumerator AnimateUI(bool enable)
    {
        string medicationUIObject = "Medication UI Parent";

        // Find the instantiated medicationUI GameObject by its name
        GameObject medicationUI = GameObject.Find(medicationUIObject);
        if (enable)
        {
            medicationUI.SetActive(true);
            LeanTween.scale(medicationUI, Vector3.one, animationDuration).setEaseOutBounce();
        }
        else
        {
            LeanTween.scale(medicationUI, Vector3.zero, animationDuration).setEaseInBounce().setOnComplete(() =>
            {
                medicationUI.SetActive(false);
            });
            yield return new WaitForSeconds(animationDuration); // Wait for the animation to complete

            needsMedication = false;
            isListenerSet = false;
            Destroy(medicationUI); // Destroy after animation
        }
    }

    public void NeedsTheMedication()
    {
        needsMedication = true;
    }

    public void NotNeedMedication()
    {
        StartCoroutine(AnimateUI(false));
        needsMedication = false;
        isListenerSet = false;
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.EnableMovement();
        }
    }

    private void SetupButtonListener()
    {
        if (!isListenerSet)
        {
            // Find the medicationUI object by name
            GameObject ui = GameObject.Find(medicationUIName);
            if (ui != null)
            {
                Button takeMedicationButton = FindButtonRecursive(ui.transform, "Take Medication Button");
                if (takeMedicationButton != null)
                {
                    Debug.Log("Set up medication button listener");
                    takeMedicationButton.onClick.AddListener(NotNeedMedication);
                    isListenerSet = true;
                }
                else
                {
                    // Debug.LogError("Take Medication Button not found in Medication UI.");
                }
            }
            else
            {
                Debug.LogError("Medication UI not found in the hierarchy.");
            }
        }
    }

    private Button FindButtonRecursive(Transform parent, string buttonName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == buttonName)
            {
                return child.GetComponent<Button>();
            }
            Button result = FindButtonRecursive(child, buttonName);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }
}

