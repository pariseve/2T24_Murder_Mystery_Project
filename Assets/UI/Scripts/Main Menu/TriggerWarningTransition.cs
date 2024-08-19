using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TriggerWarningTransition : MonoBehaviour
{
    [SerializeField] private string sceneName; // The name of the scene to load

    [SerializeField] private float sceneDuration = 10f; // The duration the scene lasts before transitioning

    [SerializeField] private float transitionDuration = 1f; // The duration of the fade transition

    [SerializeField] private CanvasGroup canvasGroup;

    private void Start()
    {
        // Start the process with the specified scene and duration
        StartCoroutine(SceneTimer());
    }

    private IEnumerator SceneTimer()
    {
        // Wait for the scene to last the specified duration
        yield return new WaitForSeconds(sceneDuration);

        // Start the scene transition
        StartTransition(sceneName);
    }

    public void StartTransition(string sceneName)
    {
        // Start the transition coroutine
        StartCoroutine(Transition(sceneName));
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Transition(string sceneName)
    {
        // Fade to black
        while (canvasGroup.alpha < 1)
        {
            Time.timeScale = 1;
            canvasGroup.alpha += Time.deltaTime / transitionDuration;
            yield return null;
        }

        // Ensure the transition image is fully opaque
        canvasGroup.alpha = 1f;

        // Load the target scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade back out
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / transitionDuration;
            yield return null;
        }

        // Ensure the transition image is fully transparent
        canvasGroup.alpha = 0f;
        StartCoroutine(DestroyObject());
    }

    private IEnumerator DestroyObject()
    {
        // Wait for a short delay before destroying the object
        yield return new WaitForSeconds(0.1f); // Adjust the delay time as needed

        // Destroy this object
        Destroy(gameObject);
    }
}

