using cherrydev;
using UnityEngine;

public class SentenceProgression : MonoBehaviour
{
    [SerializeField] private SimpleDialogBehaviour dialogBehaviour;

    private void Update()
    {
        // Check if the spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            // Call the method to progress to the next sentence node
            dialogBehaviour.ProgressToNextSentenceNode();
        }
    }
}
