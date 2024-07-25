using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    //public float transitionDuration = 1.0f; // Duration of the fade transition
    //public Image transitionImage; // Reference to the UI Image used for transition
    public string sceneName; // The name of the scene to transition to
    //private CanvasGroup canvasGroup;

    public GameObject loadingScreen;
    public Image loadingBar;

    private void Start()
    {
        // Get the CanvasGroup component attached to the transitionImage
        //canvasGroup = transitionImage.GetComponent<CanvasGroup>();
        // Ensure the transition image is fully transparent at the start
        //canvasGroup.alpha = 0f;

        // Start the transition immediately for testing purposes
        // StartTransition(sceneName); // You can uncomment this line for testing
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKey(KeyCode.Space))
        {
            StartTransition(sceneName);
        }
    }

    public void StartTransition(string sceneName)
    {
        // Start the transition coroutine
        StartCoroutine(Transition(sceneName));
    }

    private IEnumerator Transition(string sceneName)
    {
        // Fade to black
        //while (canvasGroup.alpha < 1)
        //{
        //    canvasGroup.alpha += Time.deltaTime / transitionDuration;
        //    yield return null;
        //}

        //// Ensure the transition image is fully opaque
        //canvasGroup.alpha = 1f;

        // Save the last scene before loading the new scene
        GameManager.SaveLastScene();

        // Load the target scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        loadingScreen.SetActive(true);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            float progressValue = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            loadingBar.fillAmount = progressValue;
            yield return null;
        }

        // Determine spawn position after scene is fully loaded
        Vector3 spawnPosition = DetermineSpawnPosition();

        // Instantiate the character prefab at the spawn position
        InstantiateCharacter(spawnPosition);

        // Fade back out
        //while (canvasGroup.alpha > 0)
        //{
        //    canvasGroup.alpha -= Time.deltaTime / transitionDuration;
        //    yield return null;
        //}

        // Ensure the transition image is fully transparent
        //canvasGroup.alpha = 0f;

        // Destroy this object after a short delay
        StartCoroutine(DestroyObject());
    }

    private Vector3 DetermineSpawnPosition()
    {
        // Retrieve the name of the last scene from PlayerPrefs
        string lastSceneName = PlayerPrefs.GetString("LastScene", "");
        Debug.Log("Last scene name: " + lastSceneName);

        // Find all game objects tagged as "SpawnPoint"
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        foreach (GameObject spawnPoint in spawnPoints)
        {
            Debug.Log("Spawn point name: " + spawnPoint.name);

            // Check if the spawn point name matches the expected format
            if (spawnPoint.name == lastSceneName + "SpawnPoint")
            {
                return spawnPoint.transform.position;
            }
        }

        // If no matching spawn point is found, return Vector3.zero or initial spawn position
        Debug.LogWarning("No spawn point found for scene: " + lastSceneName);
        return Vector3.zero;
    }

    private void InstantiateCharacter(Vector3 spawnPosition)
    {
        // Instantiate your character prefab at the spawn position
        // Example: Instantiate(characterPrefab, spawnPosition, Quaternion.identity);
        // Make sure to assign characterPrefab in the Inspector or load it dynamically
    }

    private IEnumerator DestroyObject()
    {
        // Wait for a short delay before destroying the object
        yield return new WaitForSeconds(0.1f); // Adjust the delay time as needed

        // Destroy this object
        Destroy(gameObject);
    }
}
