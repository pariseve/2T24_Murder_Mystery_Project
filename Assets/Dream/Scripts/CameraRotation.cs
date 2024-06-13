using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public float maxVerticalAngle = 60f;
    public float maxHorizontalAngle = 60f;

    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;

    void Start()
    {
        // Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
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
