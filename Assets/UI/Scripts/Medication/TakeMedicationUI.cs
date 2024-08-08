using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TakeMedicationUI : MonoBehaviour
{
    public static TakeMedicationUI Instance { get; private set; }

    [SerializeField] private bool needsMedication = false;
    [SerializeField] private string medicationUIName = "MedicationUI"; // The name of the instantiated UI prefab
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private TextMeshProUGUI medicationUIText;
    [SerializeField] private float scaleAmount = 0.5f;

    [SerializeField] private bool isListenerSet = false;
    private Coroutine scaleCoroutine;
    [SerializeField] private string boolName;

    private void Awake()
    {
        medicationUIText.gameObject.SetActive(false);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        GameObject medicationUI = GameObject.Find(medicationUIName);

        // Check if the boolName is true in the BoolManager
        bool isBoolTrue = BoolManager.Instance != null && BoolManager.Instance.GetBool(boolName);

        if (medicationUI != null && medicationUI.activeInHierarchy && !isListenerSet)
        {
            SetupButtonListener();
        }

        if (needsMedication && isBoolTrue)
        {
            medicationUIText.gameObject.SetActive(true);
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.DisableMovement();
            }
            if (medicationUIText != null && scaleCoroutine == null)
            {
                scaleCoroutine = StartCoroutine(ScaleText());
            }
        }
        else
        {
            if (scaleCoroutine != null)
            {
                StopCoroutine(scaleCoroutine);
                scaleCoroutine = null;
            }
            ResetSizeAndFadeOutText();
        }
        if (!isBoolTrue && medicationUI != null)
        {
            Destroy(medicationUI);
            needsMedication = false;
            isListenerSet = false;
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.EnableMovement();
            }
        }
    }

    private void SetBoolVariableTrue()
    {
        if (BoolManager.Instance != null)
        {
            BoolManager.Instance.SetBool(boolName, true);
        }
        else
        {
            Debug.LogError("BoolManager.Instance is null.");
        }
    }

    private void SetBoolVariableFalse()
    {
        if (BoolManager.Instance != null)
        {
            BoolManager.Instance.SetBool(boolName, false);
        }
        else
        {
            Debug.LogError("BoolManager.Instance is null.");
        }
    }

    private IEnumerator ScaleText()
    {
        Vector3 originalScale = medicationUIText.rectTransform.localScale;
        Vector3 maxScale = originalScale * (1 + scaleAmount);

        while (needsMedication)
        {
            // Scale up
            LeanTween.scale(medicationUIText.rectTransform, maxScale, animationDuration).setEaseInOutSine();
            yield return new WaitForSeconds(animationDuration);

            // Scale down
            LeanTween.scale(medicationUIText.rectTransform, originalScale, animationDuration).setEaseInOutSine();
            yield return new WaitForSeconds(animationDuration);
        }
    }

    private void ResetSizeAndFadeOutText()
    {
        if (medicationUIText != null)
        {
            // Reset scale
            LeanTween.scale(medicationUIText.rectTransform, Vector3.one, animationDuration).setEaseInOutSine();
            // Fade out
            LeanTween.alphaText(medicationUIText.rectTransform, 0, animationDuration).setEaseInOutSine().setOnComplete(() =>
            {
                medicationUIText.gameObject.SetActive(false);
            });
        }
    }

    public IEnumerator AnimateUI(bool enable)
    {
        string medicationUIObject = "Medication UI Parent";

        // Find the instantiated medicationUI GameObject by its name
        GameObject medicationUI = GameObject.Find(medicationUIObject);
        if (medicationUI != null)
        {
            if (enable)
            {
                medicationUI.SetActive(true);
                LeanTween.scale(medicationUI.GetComponent<RectTransform>(), Vector3.one, animationDuration).setEaseOutBounce();
            }
            else
            {
                LeanTween.scale(medicationUI.GetComponent<RectTransform>(), Vector3.zero, animationDuration).setEaseInBounce().setOnComplete(() =>
                {
                    medicationUI.SetActive(false);
                });
                yield return new WaitForSeconds(animationDuration); // Wait for the animation to complete

                needsMedication = false;
                isListenerSet = false;
                Destroy(medicationUI); // Destroy after animation
            }
        }
    }

    public void NeedsTheMedication()
    {
        needsMedication = true;
        SetBoolVariableTrue();
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
        SetBoolVariableFalse();
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

