using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSpawnManager : MonoBehaviour
{
    public GameObject characterPrefab; // Assign the character prefab in the Inspector

    public Transform initialSpawnPoint;

    private void Start()
    {
        //DontDestroyOnLoad(gameObject);

        //Vector3 initialSpawnPoint = transform.position;
        //Instantiate(characterPrefab, initialSpawnPoint, Quaternion.identity);
        SpawnCharacter();
    }

    private void SpawnCharacter()
    {
        // Determine the spawn point based on the last scene
        Vector3 spawnPosition = DetermineSpawnPosition();

        // Instantiate the character prefab at the spawn position
        Instantiate(characterPrefab, spawnPosition, Quaternion.identity);
    }

    private Vector3 DetermineSpawnPosition()
    {
        string lastSceneName = PlayerPrefs.GetString("LastScene", "");
        Debug.Log("Last scene name: " +  lastSceneName);
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        foreach (GameObject spawnPoint in spawnPoints)
        {
            Debug.Log("Spawn point name: " +  spawnPoint.name);
            
            if (spawnPoint.name == lastSceneName + "SpawnPoint") // Assumes spawn point names are "SceneNameSpawnPoint"
            {
                return spawnPoint.transform.position;
            }
        }

        Debug.LogWarning("No spawn point found for scene: " + lastSceneName);
        return initialSpawnPoint.position; // Return the initial spawn point position if no matching spawn point is found
    }

}