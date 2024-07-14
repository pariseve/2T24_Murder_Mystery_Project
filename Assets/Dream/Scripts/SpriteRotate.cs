using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteRotate : MonoBehaviour
{
    [SerializeField] private GameObject[] firstArray;
    [SerializeField] private GameObject[] secondArray;
    [SerializeField] private float rotationDuration = 1.5f;
    [SerializeField] private float initialWaitFirstArray = 2f;
    [SerializeField] private float initialWaitSecondArray = 3f;

    private Dictionary<GameObject, float> originalXRotations = new Dictionary<GameObject, float>();

    void Start()
    {
        // Store initial x rotations and set x rotation to 90 degrees for all sprites
        SetInitialRotation(firstArray);
        SetInitialRotation(secondArray);

        // Start coroutines for rotating the sprites
        StartCoroutine(RotateSprites(firstArray, initialWaitFirstArray));
        StartCoroutine(RotateSprites(secondArray, initialWaitSecondArray));
    }

    private void SetInitialRotation(GameObject[] spritesArray)
    {
        foreach (GameObject sprite in spritesArray)
        {
            float initialXRotation = sprite.transform.rotation.eulerAngles.x;
            originalXRotations[sprite] = initialXRotation;

            Vector3 rotation = sprite.transform.rotation.eulerAngles;
            rotation.x = 90;
            sprite.transform.rotation = Quaternion.Euler(rotation);
        }
    }

    private IEnumerator RotateSprites(GameObject[] spritesArray, float initialWait)
    {
        // Wait for the specified initial wait time
        yield return new WaitForSeconds(initialWait);

        // Rotate each sprite in the array over the duration specified
        foreach (GameObject sprite in spritesArray)
        {
            StartCoroutine(RotateToOriginal(sprite));
        }
    }

    private IEnumerator RotateToOriginal(GameObject sprite)
    {
        float startRotationX = sprite.transform.rotation.eulerAngles.x;
        float endRotationX = originalXRotations[sprite];
        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            float newX = Mathf.Lerp(startRotationX, endRotationX, elapsedTime / rotationDuration);
            Vector3 newRotation = sprite.transform.rotation.eulerAngles;
            newRotation.x = newX;
            sprite.transform.rotation = Quaternion.Euler(newRotation);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the rotation is exactly the original x rotation at the end
        Vector3 finalRotation = sprite.transform.rotation.eulerAngles;
        finalRotation.x = endRotationX;
        sprite.transform.rotation = Quaternion.Euler(finalRotation);
    }
}