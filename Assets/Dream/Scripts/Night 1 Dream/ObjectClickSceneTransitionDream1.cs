using UnityEngine;

public class ObjectClickSceneTransitionDream1 : MonoBehaviour
{
    [SerializeField] private UISceneTransition sceneTransition;
    [SerializeField] private string targetSceneName;
    private Dream1BoolManager dream1BoolManager;

    private void Start()
    {
        dream1BoolManager = FindObjectOfType<Dream1BoolManager>();
        if (dream1BoolManager == null)
        {
            Debug.LogError("Dream1BoolManager not found in the scene.");
        }
    }

    public void FoundAllClues()
    {
        if (dream1BoolManager != null && dream1BoolManager.brokenClock && dream1BoolManager.mirror)
        {
            if (sceneTransition != null)
            {
                sceneTransition.StartTransition(targetSceneName);
            }
            else
            {
                Debug.LogError("SceneTransition component is not assigned.");
            }
        }
        else
        {
            Debug.Log("Broken clock boolean not set to true.");
        }
    }
}
/*
private void OnMouseDown()
{
    if (Input.GetMouseButtonDown(0)) // Left mouse button clicked
    {
        if (sceneTransition != null)
        {
            sceneTransition.StartTransition(targetSceneName);
        }
        else
        {
            Debug.LogError("SceneTransition component is not assigned.");
        }
    }
}
*/


