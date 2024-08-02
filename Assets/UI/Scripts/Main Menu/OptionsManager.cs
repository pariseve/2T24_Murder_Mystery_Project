using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    // [SerializeField] private Slider sensitivitySlider;

    [SerializeField] private AudioSource[] allAudioSources; // Assign the audio sources in the inspector
    [SerializeField] private AudioSource[] sfxAudioSources;

    private void Start()
    {

        // Set the volume slider value
        float savedVolume = PlayerPrefs.GetFloat("OverallVolume", 1f);
        volumeSlider.value = savedVolume;

        // Adjust the volume based on the loaded settings
        AdjustOverallVolume();

        // Set the sensitivity slider value
        // float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 2f);
        // sensitivitySlider.value = savedSensitivity;

        // Adjust volume and mouse sensitivity based on loaded settings
        // AdjustMouseSensitivity();
    }

    public void AdjustOverallVolume()
    {
        float volume = volumeSlider.value;
        Debug.Log("Volume Adjusted: " + volume);

        // Adjust volume for all assigned audio sources
        foreach (var audioSource in allAudioSources)
        {
            audioSource.volume = volume;
        }

        // Save volume
        PlayerPrefs.SetFloat("OverallVolume", volume);
        PlayerPrefs.Save();
    }

    public void AdjustSFXVolume()
    {
        float sfxVolume = sfxVolumeSlider.value;
        Debug.Log("Volume Adjusted: " + sfxVolume);

        // Adjust volume for all assigned audio sources
        foreach (var sfxAudioSource in sfxAudioSources)
        {
            sfxAudioSource.volume = sfxVolume;
        }

        // Save volume
        PlayerPrefs.SetFloat("OverallVolume", sfxVolume);
        PlayerPrefs.Save();
    }


    /*public void AdjustMouseSensitivity()
    {
        float sensitivity = sensitivitySlider.value;
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
        PlayerPrefs.Save();
    }*/

    public void ApplyChanges()
    {
        // Save settings when apply button is clicked
        PlayerPrefs.Save();
    }
}
