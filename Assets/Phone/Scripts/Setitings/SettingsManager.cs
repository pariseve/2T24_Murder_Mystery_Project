using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [SerializeField] private GameObject settingsUI;
    [SerializeField] private Slider overallVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private AudioSource[] allAudioSources;
    private AudioSource[] sfxAudioSources;

    private float lastSFXVolume;

    [SerializeField] private float transitionDuration = 1.0f; // Duration of the fade transition

    private CanvasGroup canvasGroup;
    private GameObject transitionObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Find all audio sources with the tags "Master Volume" and "SFX Volume"
        List<AudioSource> allSourcesList = new List<AudioSource>();
        List<AudioSource> sfxSourcesList = new List<AudioSource>();

        // Search in both active scene and DontDestroyOnLoad objects
        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (obj.CompareTag("Master Volume"))
            {
                AudioSource audioSource = obj.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    allSourcesList.Add(audioSource);
                }
            }
            else if (obj.CompareTag("SFX Volume"))
            {
                AudioSource audioSource = obj.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    sfxSourcesList.Add(audioSource);
                    if (!allSourcesList.Contains(audioSource))
                    {
                        allSourcesList.Add(audioSource);
                    }
                }
            }
        }

        allAudioSources = allSourcesList.ToArray();
        sfxAudioSources = sfxSourcesList.ToArray();

        // Set the volume slider values
        float savedOverallVolume = PlayerPrefs.GetFloat("OverallVolume", 1f);
        overallVolumeSlider.value = savedOverallVolume;

        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxVolumeSlider.value = savedSFXVolume;
        lastSFXVolume = savedSFXVolume;

        // Temporarily activate sliders, add listeners, and deactivate again if they were inactive
        bool overallVolumeSliderWasInactive = !overallVolumeSlider.gameObject.activeSelf;
        bool sfxVolumeSliderWasInactive = !sfxVolumeSlider.gameObject.activeSelf;

        if (overallVolumeSliderWasInactive)
        {
            overallVolumeSlider.gameObject.SetActive(true);
        }

        overallVolumeSlider.onValueChanged.AddListener(delegate { AdjustOverallVolume(); });

        if (overallVolumeSliderWasInactive)
        {
            overallVolumeSlider.gameObject.SetActive(false);
        }

        if (sfxVolumeSliderWasInactive)
        {
            sfxVolumeSlider.gameObject.SetActive(true);
        }

        sfxVolumeSlider.onValueChanged.AddListener(delegate { AdjustSFXVolume(); });

        if (sfxVolumeSliderWasInactive)
        {
            sfxVolumeSlider.gameObject.SetActive(false);
        }

        AddBackImageListener();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            DeactivateSettings();
        }
    }

    public void StartTransition(string sceneName)
    {
        // Find the GameObject with the tag "Transition Image"
        transitionObject = GameObject.FindGameObjectWithTag("Transition Image");

        if (transitionObject != null)
        {
            // Get the CanvasGroup component from the transition object or its child
            canvasGroup = transitionObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = transitionObject.GetComponentInChildren<CanvasGroup>();
            }

            if (canvasGroup == null)
            {
                Debug.LogError("CanvasGroup component not found on transition object or its children.");
                return;
            }

            // Start the transition coroutine
            StartCoroutine(Transition(sceneName));
            DontDestroyOnLoad(transitionObject);
        }
        else
        {
            Debug.LogError("Transition Image GameObject not found.");
        }
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

        // Optionally, destroy the transition object
        StartCoroutine(DestroyObject());
    }

    private IEnumerator DestroyObject()
    {
        // Find the GameObject with the tag "Transition Image"
        transitionObject = GameObject.FindGameObjectWithTag("Transition Image");
        // Wait for a short delay before destroying the object
        yield return new WaitForSeconds(0.1f); // Adjust the delay time as needed

        if (transitionObject != null)
        {
            Destroy(transitionObject);
        }
    }

    private void DeactivateSettings()
    {
        settingsUI.SetActive(false);
    }

    public void AdjustOverallVolume()
    {
        float overallVolume = overallVolumeSlider.value;
        Debug.Log("Overall Volume Adjusted: " + overallVolume);

        // Adjust volume for all audio sources
        foreach (var audioSource in allAudioSources)
        {
            if (System.Array.Exists(sfxAudioSources, sfxAudio => sfxAudio == audioSource))
            {
                // For SFX sources, adjust based on the proportion of the last set SFX volume
                if (sfxVolumeSlider.value > 0)
                {
                    audioSource.volume = overallVolume * (sfxVolumeSlider.value / lastSFXVolume);
                }
                else
                {
                    audioSource.volume = overallVolume;
                }
            }
            else
            {
                audioSource.volume = overallVolume;
            }
        }

        // Save volume
        PlayerPrefs.SetFloat("OverallVolume", overallVolume);
        PlayerPrefs.Save();

        // Notify OptionsManager
        OptionsManager optionsManager = FindObjectOfType<OptionsManager>();
        if (optionsManager != null)
        {
            optionsManager.UpdateOverallVolume(overallVolume);
        }
    }

    public void AdjustSFXVolume()
    {
        float sfxVolume = sfxVolumeSlider.value;
        Debug.Log("SFX Volume Adjusted: " + sfxVolume);

        // Adjust volume for all SFX audio sources
        foreach (var sfxAudioSource in sfxAudioSources)
        {
            if (overallVolumeSlider.value > 0)
            {
                // Set SFX volume based on the new SFX volume
                sfxAudioSource.volume = sfxVolume * (overallVolumeSlider.value / lastSFXVolume);
            }
            else
            {
                // If overall volume is 0, set SFX volume to its last value
                sfxAudioSource.volume = sfxVolume;
            }
        }

        lastSFXVolume = sfxVolume;

        // Save volume
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();

        // Notify OptionsManager
        OptionsManager optionsManager = FindObjectOfType<OptionsManager>();
        if (optionsManager != null)
        {
            optionsManager.UpdateSFXVolume(sfxVolume);
        }
    }

    public void UpdateOverallVolume(float volume)
    {
        overallVolumeSlider.value = volume;
    }

    public void UpdateSFXVolume(float volume)
    {
        sfxVolumeSlider.value = volume;
    }

    private void AddBackImageListener()
    {
        Image backImage = settingsUI.GetComponentInChildren<Image>(true); // Search in all children
        if (backImage != null && backImage.name == "Back Button")
        {
            // Add an EventTrigger component if not already present
            EventTrigger trigger = backImage.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = backImage.gameObject.AddComponent<EventTrigger>();
            }

            // Create a new entry for Pointer Click event
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) =>
            {
                // Zoom out effect before disabling notesUI
                StartCoroutine(ZoomOutAndDisable());
            });
            trigger.triggers.Add(entry);
        }
        else
        {
            Debug.LogError("Back Button Image not found in settingsUI.");
        }
    }

    private IEnumerator ZoomOutAndDisable()
    {
        float zoomDuration = 0.5f; // Adjust duration as needed
        Vector3 originalScale = settingsUI.transform.localScale;
        Vector3 targetScale = Vector3.zero; // Zoom out to zero size

        float elapsedTime = 0f;
        while (elapsedTime < zoomDuration)
        {
            settingsUI.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it ends at the target scale
        settingsUI.transform.localScale = targetScale;

        // Disable the notesUI
        settingsUI.SetActive(false);

        // Wait for a short time
        yield return new WaitForSeconds(0.1f); // Adjust delay as needed

        // Set notesUI back to its original scale
        settingsUI.transform.localScale = originalScale;
    }
}




