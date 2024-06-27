using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public float maxVerticalAngle = 60f;
    public float maxHorizontalAngle = 60f;

    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;

    private CameraZoom cameraZoom; // Reference to CameraZoom script

    void Start()
    {
        Cursor.visible = true;

        // Get reference to CameraZoom script
        cameraZoom = GetComponent<CameraZoom>();
    }

    void Update()
    {
        if (!cameraZoom.IsZoomedIn)
        {
            // Only apply rotation when not zoomed in
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            float mouseX = horizontalInput * rotationSpeed * Time.deltaTime;
            float mouseY = verticalInput * rotationSpeed * Time.deltaTime;

            // Calculate vertical rotation
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -maxVerticalAngle, maxVerticalAngle);

            // Calculate horizontal rotation
            horizontalRotation += mouseX;
            horizontalRotation = Mathf.Clamp(horizontalRotation, -maxHorizontalAngle, maxHorizontalAngle);

            // Apply rotation to the camera
            transform.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
        }
    }

    // Function to update rotation when zoomed in
    public void UpdateRotation(Vector3 targetPosition)
    {
        // Calculate direction to target
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Calculate target rotation
        Quaternion targetRotation = Quaternion.LookRotation(direction, transform.up);

        // Smoothly rotate towards target rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // Function to reset rotation to 0, 0, 0
    public void ResetRotation()
    {
        verticalRotation = 0f;
        horizontalRotation = 0f;
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }
}


