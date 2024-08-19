using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraPathController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera; // The Cinemachine virtual camera to move
    public Transform[] cameraPoints; // Points for the camera to move to
    public float moveDuration = 5f; // Duration for each move

    private int currentPointIndex = 0;
    private bool isMoving = false;

    void Start()
    {
        if (virtualCamera == null || cameraPoints.Length == 0)
        {
            Debug.LogError("Virtual Camera or camera points are not set.");
            return;
        }

        StartMovingCamera();
    }

    void StartMovingCamera()
    {
        if (cameraPoints.Length > 0)
        {
            isMoving = true;
            StartCoroutine(MoveCameraToPoints());
        }
    }

    IEnumerator MoveCameraToPoints()
    {
        while (currentPointIndex < cameraPoints.Length)
        {
            Transform targetPoint = cameraPoints[currentPointIndex];
            yield return StartCoroutine(MoveCameraToPoint(targetPoint));
            currentPointIndex++;
        }

        // Exit play mode once the final point is reached
        if (Application.isEditor)
        {
            // UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    IEnumerator MoveCameraToPoint(Transform targetPoint)
    {
        Vector3 startPosition = virtualCamera.transform.position;
        Quaternion startRotation = virtualCamera.transform.rotation;
        Vector3 endPosition = targetPoint.position;
        Quaternion endRotation = targetPoint.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            virtualCamera.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            virtualCamera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final position and rotation
        virtualCamera.transform.position = endPosition;
        virtualCamera.transform.rotation = endRotation;
    }
}

