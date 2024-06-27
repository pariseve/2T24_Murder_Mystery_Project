using UnityEngine;
using UnityEngine.UI;

public class PhoneSystem : MonoBehaviour
{
    [SerializeField] private RawImage phoneImage;
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;
    [SerializeField] private float slideSpeed = 500f;
    [SerializeField] private GameObject applicationIcons;
    [SerializeField] private GameObject applications; // The parent object containing all application icons
    [SerializeField] private GameObject lockscreen; // The lockscreen object
    [SerializeField] private RawImage[] application; // Array of application RawImages

    private RectTransform phoneRectTransform;
    private bool isPhoneVisible = false;
    private Vector2 offScreenPosition;
    private Vector2 onScreenPosition;

    void Start()
    {
        if (phoneImage != null)
        {
            phoneRectTransform = phoneImage.GetComponent<RectTransform>();

            // Calculate the height of the canvas to correctly position the offScreenPosition
            Canvas canvas = phoneImage.canvas;
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            // Set the off-screen position (below the screen)
            offScreenPosition = new Vector2(phoneRectTransform.anchoredPosition.x, -canvasRect.rect.height);

            // Set the on-screen position (bottom of the image aligns with the bottom of the screen)
            onScreenPosition = new Vector2(phoneRectTransform.anchoredPosition.x, -250);

            // Start with the phone off-screen
            phoneRectTransform.anchoredPosition = offScreenPosition;
        }
        else
        {
            Debug.LogError("PhoneImage is not assigned!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isPhoneVisible = !isPhoneVisible;

            // Update the visibility of the applications and lockscreen
            if (!isPhoneVisible)
            {
                applicationIcons.SetActive(false);
                // applications.SetActive(false);
                lockscreen.SetActive(true);

                foreach (RawImage app in application)
                {
                    app.gameObject.SetActive(false);
                }
            }
        }

        SlidePhone();
    }

    void SlidePhone()
    {
        if (phoneRectTransform != null)
        {
            Vector2 targetPosition = isPhoneVisible ? onScreenPosition : offScreenPosition;
            phoneRectTransform.anchoredPosition = Vector2.MoveTowards(phoneRectTransform.anchoredPosition, targetPosition, slideSpeed * Time.deltaTime);
        }
    }
}



