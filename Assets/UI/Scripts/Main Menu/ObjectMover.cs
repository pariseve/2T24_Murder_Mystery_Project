using UnityEngine;
using UnityEngine.UI;

public class ObjectMover : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab; // The prefab to instantiate
    [SerializeField] private Transform startLocation; // The start location of the object
    [SerializeField] private Transform endTargetLocation; // The end target location
    [SerializeField] private float moveSpeed = 50f; // Speed at which the object moves
    [SerializeField] private Toggle toggle;

    private Coroutine moveCoroutine; // Reference to the active coroutine
    private GameObject currentObject;

    void Start()
    {
        // Subscribe to the Toggle component's onValueChanged event
        toggle.onValueChanged.AddListener(OnToggleChanged);

        // Start the process if the toggle is initially on
        if (toggle.isOn)
        {
            moveCoroutine = StartCoroutine(SpawnAndMoveObject());
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        if (isOn)
        {
            // If toggled on, start the coroutine
            moveCoroutine = StartCoroutine(SpawnAndMoveObject());
        }
        else
        {
            // If toggled off, stop the coroutine and destroy the current object
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }

            if (currentObject != null)
            {
                Destroy(currentObject);
            }
        }
    }

    private System.Collections.IEnumerator SpawnAndMoveObject()
    {
        while (true)
        {
            // Instantiate the object at the start location
            currentObject = Instantiate(objectPrefab, startLocation.position, Quaternion.identity);

            // Set the rotation of the object to keep the Y rotation at -120 degrees
            Vector3 initialRotation = currentObject.transform.eulerAngles;
            initialRotation.y = -120f;
            currentObject.transform.eulerAngles = initialRotation;

            // Calculate the direction from start to end
            Vector3 direction = (endTargetLocation.position - startLocation.position).normalized;

            // Set the object's movement
            while (currentObject != null)
            {
                // Move the object towards the target location
                currentObject.transform.position += direction * moveSpeed * Time.deltaTime;

                // Ensure the Y rotation stays at -120 degrees during movement
                Vector3 currentRotation = currentObject.transform.eulerAngles;
                currentRotation.y = -120f;
                currentObject.transform.eulerAngles = currentRotation;

                // Check if the object has passed or reached the target location
                if (Vector3.Dot(direction, endTargetLocation.position - currentObject.transform.position) <= 0)
                {
                    break;
                }

                yield return null;
            }

            // Ensure the object is destroyed if it has reached or passed the target location
            if (currentObject != null)
            {
                Destroy(currentObject);
            }

            // Wait for 10 seconds before repeating the process
            yield return new WaitForSeconds(10f);
        }
    }
}
