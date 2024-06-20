using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DialogueEditor;

public class ApartmentDay1StartDialogue : MonoBehaviour
{
    [SerializeField] private NPCConversation ApartmentDay1Start;
    [SerializeField] private Image fadeImage; // Reference to the UI Image for fading
    private const string METHOD_TRIGGERED_KEY = "ApartmentDay1StartDialogueMethodTriggered";
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
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.DisableMovement();
        }

        // Fade to black
        yield return StartCoroutine(FadeToBlack());

        // Blink three times
        for (int i = 0; i < 3; i++)
        {
            yield return StartCoroutine(FadeOut());
            yield return new WaitForSeconds(0.1f); // Short pause between blinks
            yield return StartCoroutine(FadeIn());
            yield return new WaitForSeconds(0.1f); // Short pause between blinks
        }

        // Fade out to start the game
        yield return StartCoroutine(FadeOut());

        // Start the conversation
        ConversationManager.Instance.StartConversation(ApartmentDay1Start);
        Debug.Log("Method has been triggered for the first time.");
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


