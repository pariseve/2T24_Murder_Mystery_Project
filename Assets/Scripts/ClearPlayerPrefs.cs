using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearPlayerPrefs : MonoBehaviour
{
    /*private void Awake()
    {
        PlayerPrefs.DeleteAll();
    }*/

    public void DeleteAllPlayerPrefs()
    {
        // Retrieve the current volume settings
        float savedOverallVolume = PlayerPrefs.GetFloat("OverallVolume", PlayerPrefs.GetFloat("OverallVolume"));
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", PlayerPrefs.GetFloat("SFXVolume"));

        // Delete all PlayerPrefs
        PlayerPrefs.DeleteAll();

        // Restore the volume settings
        PlayerPrefs.SetFloat("OverallVolume", savedOverallVolume);
        PlayerPrefs.SetFloat("SFXVolume", savedSFXVolume);

        // Save the restored settings
        PlayerPrefs.Save();
    }
}
