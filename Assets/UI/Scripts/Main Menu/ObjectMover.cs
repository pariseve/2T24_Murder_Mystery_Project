using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    public GameObject objectPrefab; // The prefab to instantiate
    public Transform startLocation; // The start location of the object
    public Transform endTargetLocation; // The end target location
    public float moveSpeed = 50f; // Speed at which the object moves

    void Start()
    {
        // Start the process of spawning and moving the object
        StartCoroutine(SpawnAndMoveObject());
    }

    private System.Collections.IEnumerator SpawnAndMoveObject()
    {
        while (true)
        {
            // Instantiate the object at the start location
            GameObject spawnedObject = Instantiate(objectPrefab, startLocation.position, Quaternion.identity);

            // Set the rotation of the object to keep the Y rotation at -120 degrees
            Vector3 initialRotation = spawnedObject.transform.eulerAngles;
            initialRotation.y = -120f;
            spawnedObject.transform.eulerAngles = initialRotation;

            // Calculate the direction from start to end
            Vector3 direction = (endTargetLocation.position - startLocation.position).normalized;

            // Set the object's movement
            while (spawnedObject != null)
            {
                // Move the object towards the target location
                spawnedObject.transform.position += direction * moveSpeed * Time.deltaTime;

                // Ensure the Y rotation stays at -120 degrees during movement
                Vector3 currentRotation = spawnedObject.transform.eulerAngles;
                currentRotation.y = -120f;
                spawnedObject.transform.eulerAngles = currentRotation;

                // Check if the object has passed or reached the target location
                if (Vector3.Dot(direction, endTargetLocation.position - spawnedObject.transform.position) <= 0)
                {
                    break;
                }

                yield return null;
            }

            // Ensure the object is destroyed if it has reached or passed the target location
            if (spawnedObject != null)
            {
                Destroy(spawnedObject);
            }

            // Wait for 10 seconds before repeating the process
            yield return new WaitForSeconds(10f);
        }
    }
}
