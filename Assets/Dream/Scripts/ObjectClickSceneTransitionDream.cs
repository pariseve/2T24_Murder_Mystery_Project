using UnityEngine;

public class ObjectClickSceneTransitionDream : MonoBehaviour
{
    [SerializeField] private UISceneTransition sceneTransition;
    [SerializeField] private string targetSceneName;
    [SerializeField] private string[] requiredBoolNamesTrue;

    public void FoundAllClues()
    {
        if (AllRequiredBoolsTrue())
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

    private bool AllRequiredBoolsTrue()
    {
        foreach (string boolName in requiredBoolNamesTrue)
        {
            if (!BoolManager.Instance.GetBool(boolName))
            {
                return false;
            }
        }
        return true;
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


