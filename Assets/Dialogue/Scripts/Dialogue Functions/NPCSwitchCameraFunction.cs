using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine; // Ensure you have the Cinemachine namespace included

public class NPCSwitchCameraFunction : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera mainVirtualCamera; // Main virtual camera
    [SerializeField] private CinemachineVirtualCamera altVirtualCamera; // Alternative virtual camera

    private CinemachineVirtualCamera activeCamera; // Track the active camera

    void Start()
    {
        // Ensure mainVirtualCamera is assigned
        if (mainVirtualCamera == null)
        {
            Debug.LogError("Main Virtual Camera is not assigned.");
            return;
        }

        // Start with the main virtual camera
        activeCamera = mainVirtualCamera;
    }

    // Method to reset the camera's follow target to the default object
    public void ResetCameraFollow()
    {
        if (mainVirtualCamera != null)
        {
            SwitchCameras(mainVirtualCamera);
        }
        else
        {
            Debug.LogError("Main Virtual Camera is not assigned.");
        }
    }

    // Method to switch cameras
    public void SwitchCameras(CinemachineVirtualCamera newActiveCamera)
    {
        if (newActiveCamera == null)
        {
            Debug.LogError("New Active Camera is null.");
            return;
        }

        if (newActiveCamera != activeCamera)
        {
            Debug.Log("Switching from: " + (activeCamera != null ? activeCamera.name : "None") + " to: " + newActiveCamera.name);

            // Here you are switching between different virtual cameras
            activeCamera.gameObject.SetActive(false);
            newActiveCamera.gameObject.SetActive(true);
            activeCamera = newActiveCamera;
        }
    }

    // method to set the follow target of the altVirtualCamera
    public void SetAltCameraFollowTarget(GameObject target)
    {
        if (altVirtualCamera == null)
        {
            Debug.LogError("Alternative Virtual Camera is not assigned.");
            return;
        }

        if (target == null)
        {
            Debug.LogError("Target GameObject is null.");
            return;
        }

        altVirtualCamera.Follow = target.transform;
        Debug.Log("Alternative Virtual Camera now following: " + target.name);
    }
}

