using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private Slider overallVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private AudioSource[] allAudioSources;
    private AudioSource[] sfxAudioSources;

    private float lastSFXVolume;

    private void Start()
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

    public void AdjustOverallVolume()
    {
        float overallVolume = overallVolumeSlider.value;
        Debug.Log("Overall Volume Adjusted: " + overallVolume);

        // Adjust volume for all audio sources
        foreach (var audioSource in allAudioSources)
        {
            // Apply overall volume to all audio sources
            audioSource.volume = overallVolume;
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
        float sfxVolume = sfxVolumeSlider.value;
        float overallVolume = overallVolumeSlider.value;
        Debug.Log("SFX Volume Adjusted: " + sfxVolume);

        // Adjust volume for all SFX audio sources
        foreach (var sfxAudioSource in sfxAudioSources)
        {
            // Set SFX audio source volume based on the combination of overall volume and SFX volume
            sfxAudioSource.volume = sfxVolume * overallVolume;
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
