using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine; // Ensure you have the Cinemachine namespace included

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera mainVirtualCam; // Changed type to CinemachineVirtualCamera
    public GameObject altVirtualCam;

    private GameObject activeCamera; // Track the active camera

    void Start()
    {
        activeCamera = mainVirtualCam.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SwitchCameras(altVirtualCam);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SwitchCameras(mainVirtualCam.gameObject);
        }
    }

    private void SwitchCameras(GameObject newActiveCamera)
    {
        activeCamera.SetActive(false);
        newActiveCamera.SetActive(true);
        activeCamera = newActiveCamera;
    }

    public void ShowRaymond()
    {
        SwitchCameras(altVirtualCam);
    }

    public void ShowPlayer()
    {
        SwitchCameras(mainVirtualCam.gameObject);
    }

    public void SetPlayerCharacter(GameObject character)
    {
        mainVirtualCam.Follow = character.transform;
        //mainVirtualCam.LookAt = character.transform;
    }
}
