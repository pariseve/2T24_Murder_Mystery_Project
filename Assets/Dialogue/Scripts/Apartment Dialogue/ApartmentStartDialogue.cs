using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DialogueEditor;

public class ApartmentStartDialogue : MonoBehaviour
{
    [SerializeField] private NPCConversation apartmentStart;
    [SerializeField] private Image fadeImage; // Reference to the UI Image for fading
    [SerializeField] private string METHOD_TRIGGERED_KEY = "";
    private PlayerController playerController;

    void Start()
    {
        if (PlayerPrefs.HasKey(METHOD_TRIGGERED_KEY))
        {
            // If the method has already been triggered before, start with a transparent image
            SetFadeImageAlpha(0);
        }
        else
        {
            // Otherwise, start with the image fully opaque and then trigger the method
            SetFadeImageAlpha(1);
            CheckAndTriggerMethod();
        }
    }

    private void CheckAndTriggerMethod()
    {
        if (!PlayerPrefs.HasKey(METHOD_TRIGGERED_KEY))
        {
            StartCoroutine(ExecuteFadeAndStartDialogue());

            // Set the flag to prevent this method from being triggered again
            PlayerPrefs.SetInt(METHOD_TRIGGERED_KEY, 1);
            PlayerPrefs.Save();
        }
    }

    private IEnumerator ExecuteFadeAndStartDialogue()
    {
        yield return new WaitForSeconds(0.1f); // Wait for a short delay to ensure player initialization

        // Find the PlayerController
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.DisableMovement();
        }
        else
        {
            Debug.LogWarning("PlayerController not found immediately. Waiting for it to be initialized.");
            yield return StartCoroutine(WaitForPlayerInitialization());
            playerController = FindObjectOfType<PlayerController>(); // Try finding again
            if (playerController != null)
            {
                playerController.DisableMovement();
            }
            else
            {
                Debug.LogError("PlayerController not found.");
                yield break; // Exit coroutine if PlayerController still not found
            }
        }

        // Fade to black
        yield return StartCoroutine(FadeToBlack());

        // Blink three times
        for (int i = 0; i < 1; i++)
        {
            yield return StartCoroutine(FadeOut());
            yield return new WaitForSeconds(0.1f); // Short pause between blinks
            yield return StartCoroutine(FadeIn());
            yield return new WaitForSeconds(0.1f); // Short pause between blinks
        }

        // Fade out to start the game
        yield return StartCoroutine(FadeOut());

        // Start the conversation
        ConversationManager.Instance.StartConversation(apartmentStart);
        Debug.Log("Method has been triggered for the first time.");
    }

    private IEnumerator WaitForPlayerInitialization()
    {
        yield return new WaitForSeconds(1f); // Adjust delay as needed
    }

    private IEnumerator FadeToBlack()
    {
        for (float alpha = 0; alpha <= 1; alpha += Time.deltaTime)
        {
            SetFadeImageAlpha(alpha);
            yield return null;
        }
        SetFadeImageAlpha(1);
    }

    private IEnumerator FadeOut()
    {
        for (float alpha = 1; alpha >= 0; alpha -= Time.deltaTime)
        {
            SetFadeImageAlpha(alpha);
            yield return null;
        }
        SetFadeImageAlpha(0);
    }

    private IEnumerator FadeIn()
    {
        for (float alpha = 0; alpha <= 1; alpha += Time.deltaTime)
        {
            SetFadeImageAlpha(alpha);
            yield return null;
        }
        SetFadeImageAlpha(1);
    }

    private void SetFadeImageAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }

    // You can call this method if you ever need to reset the flag (for testing purposes, etc.)
    public void ResetTriggerFlag()
    {
        PlayerPrefs.DeleteKey(METHOD_TRIGGERED_KEY);
    }
}



