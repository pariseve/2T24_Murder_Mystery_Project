using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private Slider overallVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private AudioSource[] allAudioSources;
    private AudioSource[] sfxAudioSources;

    [SerializeField] private GameObject[] audioManager;

    private float lastSFXVolume;

    private void Awake()
    {
        foreach (GameObject prefab in audioManager)
        {
            if (prefab != null)
            {
                // Check if the prefab already exists in the scene or DontDestroyOnLoad objects
                if (!IsObjectAlreadyInScene(prefab.name))
                {
                    Instantiate(prefab);
                }
                else
                {
                    Debug.Log($"Object '{prefab.name}' already exists, skipping instantiation.");
                }
            }
            else
            {
                Debug.LogWarning("Prefab is null.");
            }
        }


    }

    private bool IsObjectAlreadyInScene(string prefabName)
    {
        // Check in the current scene
        GameObject existingObject = GameObject.Find(prefabName);
        if (existingObject != null)
        {
            return true;
        }

        // Check in DontDestroyOnLoad objects
        var dontDestroyOnLoadObjects = FindObjectsOfType<GameObject>(true)
            .Where(go => go.scene.name == "DontDestroyOnLoad")
            .ToArray();

        if (dontDestroyOnLoadObjects.Any(obj => obj.name == prefabName))
        {
            return true;
        }

        return false;
    }

    private void Start()
    {
        // Initialize references
        UpdateAudioSourceReferences();

        // Set the volume slider values
        float savedOverallVolume = PlayerPrefs.GetFloat("OverallVolume", 1f);
        overallVolumeSlider.value = savedOverallVolume;

        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxVolumeSlider.value = savedSFXVolume;
        lastSFXVolume = savedSFXVolume;

        // Adjust the volume based on the loaded settings
        AdjustOverallVolume();
        AdjustSFXVolume();

        // Add listeners to the sliders
        overallVolumeSlider.onValueChanged.AddListener(delegate { AdjustOverallVolume(); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { AdjustSFXVolume(); });
    }

    public void UpdateOverallVolume(float volume)
    {
        overallVolumeSlider.value = volume;
    }

    public void UpdateSFXVolume(float volume)
    {
        sfxVolumeSlider.value = volume;
    }

    private void UpdateAudioSourceReferences()
    {
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
    }

    public void AdjustOverallVolume()
    {
        // Ensure references are still valid
        if (allAudioSources == null || allAudioSources.Length == 0 || allAudioSources.Any(source => source == null))
        {
            UpdateAudioSourceReferences();
        }

        float overallVolume = overallVolumeSlider.value;
        Debug.Log("Overall Volume Adjusted: " + overallVolume);

        // Adjust volume for all audio sources
        foreach (var audioSource in allAudioSources)
        {
            if (audioSource != null)
            {
                // Apply overall volume to all audio sources
                audioSource.volume = overallVolume;
            }
        }

        // Save volume
        PlayerPrefs.SetFloat("OverallVolume", overallVolume);
        PlayerPrefs.Save();

        // Notify SettingsManager
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.UpdateOverallVolume(overallVolume);
        }
    }

    public void AdjustSFXVolume()
    {
        // Ensure references are still valid
        if (sfxAudioSources == null || sfxAudioSources.Length == 0 || sfxAudioSources.Any(source => source == null))
        {
            UpdateAudioSourceReferences();
        }

        float sfxVolume = sfxVolumeSlider.value;
        float overallVolume = overallVolumeSlider.value;
        Debug.Log("SFX Volume Adjusted: " + sfxVolume);

        // Adjust volume for all SFX audio sources
        foreach (var sfxAudioSource in sfxAudioSources)
        {
            if (sfxAudioSource != null)
            {
                // Set SFX audio source volume based on the combination of overall volume and SFX volume
                sfxAudioSource.volume = sfxVolume * overallVolume;
            }
        }

        lastSFXVolume = sfxVolume;

        // Save volume
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();

        // Notify SettingsManager
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.UpdateSFXVolume(sfxVolume);
        }
    }

    public void ApplyChanges()
    {
        // Save settings when apply button is clicked
        PlayerPrefs.Save();
    }
}
