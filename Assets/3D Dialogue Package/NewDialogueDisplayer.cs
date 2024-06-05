// NewDialogDisplayer.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace cherrydev
{
    public class NewDialogDisplayer : MonoBehaviour
    {
        [SerializeField] private SimpleDialogBehaviour dialogBehaviour;
        [SerializeField] private GameObject dialogPanel;
        [SerializeField] private TextMeshPro dialogText;

        private void OnEnable()
        {
            dialogBehaviour.OnDialogTextSkipped += ShowFullDialogText;
            dialogBehaviour.OnSentenceNodeActive += SetupDialogText;
            dialogBehaviour.OnDialogEnded += DisableDialogPanel; // Subscribe to the event
        }

        private void OnDisable()
        {
            dialogBehaviour.OnDialogTextSkipped -= ShowFullDialogText;
            dialogBehaviour.OnSentenceNodeActive -= SetupDialogText;
            dialogBehaviour.OnDialogEnded -= DisableDialogPanel; // Unsubscribe from the event
        }

        public void ShowDialogPanel()
        {
            dialogPanel.SetActive(true);
        }

        public void DisableDialogPanel()
        {
            dialogPanel.SetActive(false);
        }

        public void SetupDialogText(string dialogText)
        {
            this.dialogText.text = dialogText;
        }

        public void ShowFullDialogText(string dialogText)
        {
            this.dialogText.text = dialogText;
        }
    }
}
