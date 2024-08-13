using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 2f; // Slower zoom speed
    [SerializeField] private float zoomFactor = 2f; // Distance multiplier for zooming in
    [SerializeField] private LayerMask zoomLayer;

    [SerializeField] private KeyCode startKey = KeyCode.E;

    private Camera cam;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    [SerializeField] private bool isZoomedIn = false;
    [SerializeField] private bool zoomEnabled = true; // New flag to control zoom functionality
    [SerializeField] public bool isZooming = false; // New flag to indicate if zoom is in progress

    public bool isZoomedInMirror = false;
    public bool IsZoomedIn { get { return isZoomedIn; } }

    // Reference to CameraRotation script
    private CameraRotation cameraRotation;

    void Start()
    {
        cam = GetComponent<Camera>();
        originalPosition = cam.transform.position;
        originalRotation = cam.transform.rotation;

        // Get reference to CameraRotation script
        cameraRotation = GetComponent<CameraRotation>();
    }

    void Update()
    {
        if (!zoomEnabled || isZooming) return; // Skip zoom logic if zoom is disabled or zoom is in progress

        if (Input.GetKeyDown(startKey) && !isZoomedIn)
        {
            ZoomInToObject();
        }
        else if (Input.GetKeyDown(startKey) && isZoomedIn)
        {
            ZoomOut();
        }
    }

    public void ZoomInToObject()
    {
        if (isZoomedIn || !zoomEnabled || isZooming) return;

        // Perform a raycast to see what the player clicked on
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, zoomLayer))
        {

            if (hit.transform.CompareTag("mirror"))
            {
                isZoomedInMirror = true;
                zoomFactor = 2.5f;
            }
            else
            {
                isZoomedInMirror = false;
                zoomFactor = 1f;
            }
            // Check if there are any colliders between the camera and the hit point
            Vector3 direction = hit.point - cam.transform.position;
            float distance = direction.magnitude;
            direction.Normalize();

            RaycastHit[] hits = Physics.RaycastAll(cam.transform.position, direction, distance, ~zoomLayer);
            bool isHit = false;

            foreach (RaycastHit obstacleHit in hits)
            {
                if (obstacleHit.transform != hit.transform)
                {
                    isHit = true;
                    break;
                }
            }

            if (!isHit)
            {
                Debug.Log("about to zoom in");
                // Zoom in towards the object that was clicked
                StartCoroutine(ZoomToPosition(hit.transform.position));
                isZoomedIn = true;
            }
            else
            {
                Debug.Log("Object is obscured by other colliders. Cannot zoom in.");
            }
        }
    }

    public void ZoomOut()
    {
        if (!isZoomedIn || !zoomEnabled || isZooming) return;
        StartCoroutine(ZoomOutCoroutine());
        
    }

    private IEnumerator ZoomToPosition(Vector3 targetPosition)
    {
        isZooming = true; // Set zooming flag to true
        float progress = 0f;
        Vector3 startPosition = cam.transform.position;
        Quaternion startRotation = cam.transform.rotation; // Store the current rotation as the start rotation
        Vector3 direction = (targetPosition - cam.transform.position).normalized;

        // Calculate target rotation without changing the current rotation
        Quaternion targetRotation = Quaternion.LookRotation(direction, cam.transform.up);

        while (progress < 1f)
        {
            progress += Time.deltaTime * zoomSpeed;
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            // Calculate the new position
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition - direction * zoomFactor, smoothProgress);

            // Apply position
            cam.transform.position = newPosition;

            // Apply rotation smoothly
            cam.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, smoothProgress);

            // Center the object horizontally on screen
            Vector3 screenPoint = cam.WorldToViewportPoint(targetPosition);
            Vector3 offset = new Vector3(0.5f - screenPoint.x, 0f, 0f);
            offset = cam.transform.TransformDirection(offset);
            cam.transform.position += offset;

            yield return null;
        }

        // Final position and rotation assignment
        cam.transform.position = targetPosition - direction * zoomFactor;
        cam.transform.rotation = targetRotation; // Assign the target rotation directly at the end

        // Re-enable CameraRotation script after zooming
        cameraRotation.enabled = true;

        // Update zoom state
        isZoomedIn = true;
        isZooming = false; // Reset zooming flag
    }

    private IEnumerator ZoomOutCoroutine()
    {
        if (isZoomedInMirror)
        {
            isZoomedInMirror = false;
        }
        isZooming = true; // Set zooming flag to true
        float progress = 0f;
        Vector3 startPosition = cam.transform.position;
        Quaternion startRotation = cam.transform.rotation;

        while (progress < 1f)
        {
            progress += Time.deltaTime * zoomSpeed;
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            // Calculate the new position and rotation
            Vector3 newPosition = Vector3.Lerp(startPosition, originalPosition, smoothProgress);
            Quaternion newRotation = Quaternion.Lerp(startRotation, originalRotation, smoothProgress);

            // Apply position and rotation
            cam.transform.position = newPosition;
            cam.transform.rotation = newRotation;

            yield return null;
        }

        // Final position and rotation assignment
        cam.transform.position = originalPosition;
        cam.transform.rotation = originalRotation;

        // Update zoom state
        isZoomedIn = false;
        isZooming = false; // Reset zooming flag

        // Re-enable CameraRotation script after zooming out
        cameraRotation.enabled = true;

        // Reset rotation in CameraRotation
        cameraRotation.ResetRotation();
    }

    // Method to enable zooming
    public void EnableZoom()
    {
        zoomEnabled = true;
    }

    // Method to disable zooming
    public void DisableZoom()
    {
        Debug.Log("disable zoom");
        zoomEnabled = false;
    }
}