using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ApplicationManager : MonoBehaviour
{
    [SerializeField] private GameObject applicationIcons; // The parent object containing all application icons
    [SerializeField] private RawImage fingerprintImage;
    [SerializeField] private GameObject lockscreen;
    [SerializeField] private float longPressDuration = 1f;
    [SerializeField] private float bounceDuration = 0.5f;
    [SerializeField] private Vector3 bounceScale = new Vector3(1.2f, 1.2f, 1.2f);
    [SerializeField] private RawImage[] applications; // Array of application RawImages
    [SerializeField] private RawImage[] appIcons; // Array of RawImages corresponding to application icons

    private bool isLongPressing = false;
    private float pressTime = 0f;


    void Start()
    {
        // Ensure the applications are disabled at the start
        applicationIcons.SetActive(false);

        // Add Event Triggers to the RawImage for detecting long press
        fingerprintImage.gameObject.AddComponent<Button>().onClick.AddListener(() => { });

        // Add click event handlers to application icons
        for (int i = 0; i < appIcons.Length; i++)
        {
            int index = i; // Capture the current value of i in the closure
            EventTrigger trigger = appIcons[i].gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => { OpenApplication(index); });
            trigger.triggers.Add(entry);
        }

        foreach (RawImage app in applications)
        {
            app.gameObject.SetActive(false);
        }

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(fingerprintImage.rectTransform, Input.mousePosition, null, out Vector2 localPoint);
            if (fingerprintImage.rectTransform.rect.Contains(localPoint))
            {
                if (!isLongPressing)
                {
                    isLongPressing = true;
                    pressTime = Time.time;
                }
                else
                {
                    if (Time.time - pressTime >= longPressDuration)
                    {
                        ShowApplications();
                        isLongPressing = false;

                        if (lockscreen != null)
                        {
                            lockscreen.SetActive(false);
                        }
                    }
                }
            }
        }
        else
        {
            isLongPressing = false;
        }
    }

    void ShowApplications()
    {
        applicationIcons.SetActive(true);

        // Disable all application RawImages
        foreach (RawImage app in applications)
        {
            app.gameObject.SetActive(false);
        }

        for (int i = 0; i < appIcons.Length; i++)
        {
            // Ensure the index is valid and less than the length of the applications array
            if (i >= 0 && i < applications.Length)
            {
                StartCoroutine(BounceAnimation(appIcons[i].transform));
            }
        }
    }

    System.Collections.IEnumerator BounceAnimation(Transform appIcon)
    {
        Vector3 originalScale = appIcon.localScale;
        Vector3 targetScale = originalScale + bounceScale;
        float elapsedTime = 0f;

        // Scale up
        while (elapsedTime < bounceDuration)
        {
            appIcon.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / bounceDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Scale down
        elapsedTime = 0f;
        while (elapsedTime < bounceDuration)
        {
            appIcon.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / bounceDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        appIcon.localScale = originalScale;
    }

    void OpenApplication(int index)
    {
        // Disable all application RawImages
        foreach (RawImage app in applications)
        {
            app.gameObject.SetActive(false);
        }

        // Disable all application icons
        foreach (RawImage icon in appIcons)
        {
            // icon.gameObject.SetActive(false);
        }

        // Enable the selected application
        if (index >= 0 && index < applications.Length)
        {
            applications[index].gameObject.SetActive(true);
        }

        // Enable the selected application icon and apply zoom-in transition
        if (index >= 0 && index < appIcons.Length)
        {
            StartCoroutine(ZoomInTransition(applications[index].transform));
        }
    }

    System.Collections.IEnumerator ZoomInTransition(Transform application)
    {
        Vector3 originalScale = application.localScale;
        Vector3 targetScale = originalScale;
        float elapsedTime = 0f;

        // Scale up
        while (elapsedTime < bounceDuration)
        {
            application.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsedTime / bounceDuration); // Start from zero size
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set to the original scale
        application.localScale = originalScale;
    }
}



