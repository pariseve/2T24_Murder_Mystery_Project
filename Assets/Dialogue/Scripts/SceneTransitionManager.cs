using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using Cinemachine;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Transition Settings")]
    [SerializeField] private float transitionDuration = 1.0f; // Duration of the fade transition
    [SerializeField] private Image transitionImage; // Reference to the UI Image used for transition
    [SerializeField] private float textFadeDuration = 1.0f; // Duration of text fade in/out

    [Header("Camera Movement Settings")]
    [SerializeField] private float cameraMoveDuration = 3.0f; // Duration of camera movement

    [Header("Scene Management")]
    [SerializeField] private string[] scenesToLoad; // Array of scenes to load in sequence
    [SerializeField] private string mainMenuScene; // Name of the main menu scene
    [TextArea(3, 10)]
    [SerializeField] private string[] creditsTextForScenes; // Array of credits text for each scene

    private CanvasGroup canvasGroup;
    private int currentSceneIndex = 0;
    private bool isTransitioning = false;
    private TextMeshProUGUI creditsText;

    private void Awake()
    {
        // Ensure there is only one instance of this script (singleton pattern)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Get the CanvasGroup component attached to the transitionImage
        canvasGroup = transitionImage.GetComponent<CanvasGroup>();

        // Ensure the transition image is fully transparent at the start
        canvasGroup.alpha = 0f;

        // Find the Credits Text as a child of this GameObject
        creditsText = GetComponentInChildren<TextMeshProUGUI>();

        if (creditsText == null)
        {
            Debug.LogError("Credits Text (TextMeshProUGUI) is not assigned or found as a child of the GameObject.");
        }
    }

    public void StartTransitionSequence()
    {
        if (!isTransitioning && currentSceneIndex < scenesToLoad.Length)
        {
            StartCoroutine(TransitionAndMoveCamera(scenesToLoad[currentSceneIndex], creditsTextForScenes[currentSceneIndex]));
        }
        else if (!isTransitioning)
        {
            StartCoroutine(TransitionToMainMenu());
        }
    }

    private IEnumerator TransitionAndMoveCamera(string sceneName, string creditsTextContent)
    {
        isTransitioning = true;

        // Fade in the image to black
        yield return StartCoroutine(FadeImage(true));

        // Load the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Automatically find start and finish positions and the virtual camera
        Transform startTarget = GameObject.FindGameObjectWithTag("Start Position")?.transform;
        Transform finishTarget = GameObject.FindGameObjectWithTag("End Position")?.transform;
        CinemachineVirtualCamera virtualCamera = GameObject.FindGameObjectWithTag("Credits Camera")?.GetComponent<CinemachineVirtualCamera>();

        // If any of the objects are not found, log a warning and skip this scene
        if (startTarget == null || finishTarget == null || virtualCamera == null)
        {
            Debug.LogWarning("One or more tagged objects not found. Skipping this scene.");
            currentSceneIndex++;
            isTransitioning = false;
            StartTransitionSequence();
            yield break;
        }

        // Set the camera to the start position
        virtualCamera.transform.position = startTarget.position;
        virtualCamera.transform.rotation = startTarget.rotation;

        // Set the credits text for this scene
        creditsText.text = creditsTextContent;

        // Fade out the image to reveal the scene
        yield return StartCoroutine(FadeImage(false));

        // Fade in the text
        yield return StartCoroutine(FadeText(creditsText, true));

        // Move the camera from start to finish
        yield return StartCoroutine(MoveCamera(virtualCamera, startTarget, finishTarget));

        // Fade out the text
        yield return StartCoroutine(FadeText(creditsText, false));

        // Fade in the image to black again
        yield return StartCoroutine(FadeImage(true));

        // Move to the next scene in the list
        currentSceneIndex++;
        isTransitioning = false;
        StartTransitionSequence();
    }

    private IEnumerator TransitionToMainMenu()
    {
        // Fade in the image to black
        yield return StartCoroutine(FadeImage(true));

        // Load the main menu scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainMenuScene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade out the image to reveal the main menu
        yield return StartCoroutine(FadeImage(false));
        Destroy(gameObject);
    }

    private IEnumerator FadeImage(bool fadeIn)
    {
        float targetAlpha = fadeIn ? 1.0f : 0.0f;
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / transitionDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    private IEnumerator FadeText(TextMeshProUGUI text, bool fadeIn)
    {
        float targetAlpha = fadeIn ? 1.0f : 0.0f;
        float startAlpha = text.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < textFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            text.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / textFadeDuration);
            yield return null;
        }

        text.alpha = targetAlpha;
    }

    private IEnumerator MoveCamera(CinemachineVirtualCamera virtualCamera, Transform start, Transform finish)
    {
        float elapsedTime = 0f;

        while (elapsedTime < cameraMoveDuration)
        {
            float t = elapsedTime / cameraMoveDuration;
            virtualCamera.transform.position = Vector3.Lerp(start.position, finish.position, t);
            virtualCamera.transform.rotation = Quaternion.Lerp(start.rotation, finish.rotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        virtualCamera.transform.position = finish.position;
        virtualCamera.transform.rotation = finish.rotation;
    }
}
