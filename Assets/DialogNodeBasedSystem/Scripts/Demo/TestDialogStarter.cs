using UnityEngine;
using cherrydev;

public class TestDialogStarter : MonoBehaviour
{
    [SerializeField] private SimpleDialogBehaviour dialogBehaviour;
    [SerializeField] private DialogNodeGraph dialogGraph;

    private void Start()
    {
        dialogBehaviour.BindExternalFunction("Test", DebugExternal);
        Debug.Log("Text is displaying");

        dialogBehaviour.StartDialog(dialogGraph);
    }

    private void DebugExternal()
    {
        Debug.Log("External function works!");
    }
}