using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string LastSceneKey = "LastScene";
    // Start is called before the first frame update
    void Awake()
    {
        Inventory.instance.Clear();
    }

    private void OnApplicationQuit()
    {
        Inventory.instance.SaveInventory();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Inventory.instance.SaveInventory();
        }
    }

    public void ChangeScene(string sceneName)
    {
        Inventory.instance.SaveInventory();
        SceneManager.LoadScene(sceneName);
    }
    public static void SaveLastScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString(LastSceneKey, currentSceneName);
        PlayerPrefs.Save(); // Ensure PlayerPrefs changes are saved immediately
    }
}
