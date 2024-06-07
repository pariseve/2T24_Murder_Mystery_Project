using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string LastSceneKey = "LastScene";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public static void SaveLastScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString(LastSceneKey, currentSceneName);
        PlayerPrefs.Save(); // Ensure PlayerPrefs changes are saved immediately
    }
}
