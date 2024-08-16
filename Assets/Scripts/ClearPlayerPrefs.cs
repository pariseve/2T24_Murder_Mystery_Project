using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearPlayerPrefs : MonoBehaviour
{
    /*private void Awake()
    {
        PlayerPrefs.DeleteAll();
    }*/

    private void Awake()
    {
        // Unlock the cursor from the center of the screen
        Cursor.lockState = CursorLockMode.None;

        // Show the cursor
        Cursor.visible = true;
    }

    public void DeleteAllPlayerPrefs()
    {
        Debug.Log("clear all player prefs");
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
