using UnityEngine;
using System.Collections;
using DialogueEditor;
using System.Collections.Generic;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 2f; // Slower zoom speed
    [SerializeField] private float zoomFactor = 2f; // Distance multiplier for zooming in
    [SerializeField] private LayerMask zoomLayer;

    [SerializeField] private KeyCode startKey = KeyCode.E;

    private Camera cam;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isZoomedIn = false;

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
        if (isZoomedIn) return;

        // Perform a raycast to see what the player clicked on
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, zoomLayer))
        {
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
        if (!isZoomedIn) return;
        StartCoroutine(ZoomOutCoroutine());
    }

    private IEnumerator ZoomToPosition(Vector3 targetPosition)
    {
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
    }

    private IEnumerator ZoomOutCoroutine()
    {
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

        // Re-enable CameraRotation script after zooming out
        cameraRotation.enabled = true;

        // Reset rotation in CameraRotation
        cameraRotation.ResetRotation();
    }
}