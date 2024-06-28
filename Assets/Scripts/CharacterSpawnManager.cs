using UnityEngine;

public class CharacterSpawnManager : MonoBehaviour
{
    public GameObject characterPrefab; // Assign the character prefab in the Inspector
    public Transform initialSpawnPoint;
    private CameraManager cameraManager;

    private void Start()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        SpawnCharacter();
    }

    private void SpawnCharacter()
    {
        // Determine the spawn point based on the last scene
        Vector3 spawnPosition = DetermineSpawnPosition();

        // Instantiate the character prefab at the spawn position
        GameObject characterInstance = Instantiate(characterPrefab, spawnPosition, Quaternion.identity);

        if (cameraManager != null)
        {
            cameraManager.SetPlayerCharacter(characterInstance);
        }
        else
        {
            Debug.LogWarning("CameraManager not found in the scene. Skipping camera setup.");
        }
    }

    private Vector3 DetermineSpawnPosition()
    {
        string lastSceneName = PlayerPrefs.GetString("LastScene", "");
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        foreach (GameObject spawnPoint in spawnPoints)
        {
            if (spawnPoint.name == lastSceneName + "SpawnPoint") // Assumes spawn point names are "SceneNameSpawnPoint"
            {
                return spawnPoint.transform.position;
            }
        }

        return initialSpawnPoint.position; // Return the initial spawn point position if no matching spawn point is found
    }
}
