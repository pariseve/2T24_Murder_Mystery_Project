using UnityEngine;
using System.Collections;
using DialogueEditor;

public class CameraZoom : MonoBehaviour
{
    public float zoomSpeed = 10f;
    public float zoomFactor = 2f; // Distance multiplier for zooming in
    public LayerMask zoomLayer;

    private Camera cam;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isZoomedIn = false;

    private NPCConversation npcConversation;
    private ConversationManager conversationManager;

    void Start()
    {
        cam = GetComponent<Camera>();
        originalPosition = cam.transform.position;
        originalRotation = cam.transform.rotation;

        // Find the NPCConversation component in the scene
        npcConversation = FindObjectOfType<NPCConversation>();
        conversationManager = FindObjectOfType<ConversationManager>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ZoomInToObject();
        }
        else if (Input.GetMouseButtonDown(1) && !npcConversation.isDialogueActive)
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

        // Zoom back out to the original position
        StartCoroutine(ZoomOutCoroutine());
    }

    private IEnumerator ZoomToPosition(Vector3 targetPosition)
    {
        float progress = 0f;
        Vector3 startPosition = cam.transform.position;
        Quaternion startRotation = cam.transform.rotation;
        Vector3 direction = (targetPosition - cam.transform.position).normalized;

        // Calculate the target rotation to look at the object
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        while (progress < 1f)
        {
            cam.transform.position = Vector3.Lerp(startPosition, targetPosition - direction * zoomFactor, progress);
            cam.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, progress);
            progress += Time.deltaTime * zoomSpeed;
            yield return null;
        }

        cam.transform.position = targetPosition - direction * zoomFactor;
        cam.transform.rotation = targetRotation;

        // After reaching the target position and rotation, ensure the object is centered horizontally on screen
        CenterObjectInView(targetPosition);
    }

    private IEnumerator ZoomOutCoroutine()
    {
        float progress = 0f;
        Vector3 startPosition = cam.transform.position;
        Quaternion startRotation = cam.transform.rotation;

        while (progress < 1f)
        {
            cam.transform.position = Vector3.Lerp(startPosition, originalPosition, progress);
            cam.transform.rotation = Quaternion.Lerp(startRotation, originalRotation, progress);
            progress += Time.deltaTime * zoomSpeed;
            yield return null;
        }

        cam.transform.position = originalPosition;
        cam.transform.rotation = originalRotation;

        // After zooming out, ensure the object is centered horizontally on screen
        if (isZoomedIn)
        {
            CenterObjectInView(originalPosition);
            isZoomedIn = false;
        }
    }

    private void CenterObjectInView(Vector3 targetPosition)
    {
        // Convert the target position from world space to viewport space
        Vector3 screenPoint = cam.WorldToViewportPoint(targetPosition);

        // Calculate the offset to center the target horizontally on screen
        Vector3 offset = new Vector3(0.5f - screenPoint.x, 0f, 0f);

        // Convert the offset to world space
        offset = cam.transform.TransformDirection(offset);

        // Apply the offset to the camera position
        cam.transform.position += offset;
    }
}




