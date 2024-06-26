using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject mainVirtualCam;
    public GameObject altVirtualCam;

    private GameObject activeCamera; // Track the active camera

    void Start()
    {
        activeCamera = mainVirtualCam;
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
            SwitchCameras(mainVirtualCam);
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
        SwitchCameras(mainVirtualCam);
    }
}