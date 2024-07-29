using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialControl : MonoBehaviour
{
    [SerializeField] private float minValue = 0f;
    [SerializeField] private float maxValue = 100f;
    [SerializeField] public float currentValue = 50f; // Initial value
    [SerializeField] private TextMeshProUGUI valueText; // Reference to the Text component
    [SerializeField] private Image dialImage; // The image to interact with
    [SerializeField] private float stepSize = 0.5f; // Step size for increments/decrements

    private RectTransform rectTransform;
    private bool isDragging = false;
    private Vector2 dialCenterPos;
    private float previousAngle;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateDial(); // Initialize the display text
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check if the mouse is over the interactive image
            if (RectTransformUtility.RectangleContainsScreenPoint(dialImage.rectTransform, Input.mousePosition))
            {
                isDragging = true;
                dialCenterPos = RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);
                previousAngle = GetAngleFromMousePosition(Input.mousePosition);
                Debug.Log($"Started Dragging: Initial Value: {currentValue}, Initial Angle: {previousAngle}");
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            Debug.Log($"Stopped Dragging: Final Value: {currentValue}");
        }

        if (isDragging)
        {
            Vector2 currentMousePos = Input.mousePosition;
            float currentAngle = GetAngleFromMousePosition(currentMousePos);

            // Calculate the angle difference considering the dial's circular nature
            float angleDifference = currentAngle - previousAngle;
            if (Mathf.Abs(angleDifference) > 180)
            {
                if (angleDifference > 0) angleDifference -= 360;
                else angleDifference += 360;
            }

            // Determine if the direction of spinning is forward or backward
            float direction = Mathf.Sign(angleDifference);

            // Calculate the potential new value based on angle difference and step size
            float valueChange = Mathf.Abs(angleDifference) * ((maxValue - minValue) / 360f);
            float stepCount = Mathf.Round(valueChange / stepSize);
            float potentialValue = currentValue + (direction * stepCount * stepSize);

            // Clamp the potential value within minValue and maxValue
            currentValue = Mathf.Clamp(potentialValue, minValue, maxValue);

            // Apply new value and update dial
            if (!Mathf.Approximately(currentValue, previousAngle))
            {
                UpdateDial();
                Debug.Log($"Dragging: Current Angle: {currentAngle}, Angle Difference: {angleDifference}, New Value: {currentValue}");
            }

            // Update previousAngle for the next frame
            previousAngle = currentAngle;
        }
    }

    private float GetAngleFromMousePosition(Vector2 mousePosition)
    {
        Vector2 direction = mousePosition - dialCenterPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // Adjust to make 0 at the top

        // Normalize angle to a value between 0 and 360
        if (angle < 0) angle += 360;

        // Invert the angle to match the dial rotation direction
        angle = (360 - angle) % 360;

        Debug.Log($"Mouse Position: {mousePosition}, Calculated Angle: {angle}");
        return angle;
    }

    public void UpdateDial()
    {
        float normalizedValue = (currentValue - minValue) / (maxValue - minValue);
        float rotationAngle = Mathf.Lerp(0, 360, normalizedValue);
        rectTransform.rotation = Quaternion.Euler(0, 0, -rotationAngle); // Adjust rotation based on value

        if (valueText != null)
        {
            valueText.text = $"{currentValue:F2}"; // Update the text with formatted value
        }
    }
}

