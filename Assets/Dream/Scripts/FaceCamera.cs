using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] private LayerMask layersToFaceCamera;

    void Update()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        if (mainCamera != null)
        {
            // Find all objects in the scene
            GameObject[] allObjects = FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                // Check if the object's layer is included in layersToFaceCamera but not the Default layer
                int objectLayerMask = 1 << obj.layer;
                int defaultLayerMask = 1 << LayerMask.NameToLayer("Default");

                if ((objectLayerMask & layersToFaceCamera) != 0 && (objectLayerMask & defaultLayerMask) == 0)
                {
                    Vector3 lookDirection = mainCamera.transform.position - obj.transform.position;
                    obj.transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                    Debug.Log(obj.name + " rotated towards the camera.");
                }
            }
        }
        else
        {
            Debug.LogWarning("No object tagged as 'MainCamera' found in the scene.");
        }
    }
}




