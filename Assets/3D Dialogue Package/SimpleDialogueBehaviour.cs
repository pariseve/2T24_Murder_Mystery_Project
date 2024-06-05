// SimpleDialogBehaviour.cs

using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace cherrydev
{
    public class SimpleDialogBehaviour : MonoBehaviour
    {
        [SerializeField] private float dialogCharDelay;
        [SerializeField] private UnityEvent onDialogStarted;
        [SerializeField] private UnityEvent onDialogFinished;

        private DialogNodeGraph currentNodeGraph;
        private Node currentNode;

        private bool isDialogStarted;
        private bool isCurrentSentenceSkipped;

        public event UnityAction<string> OnDialogTextSkipped;
        public event UnityAction<string> OnSentenceNodeActive;
        public event UnityAction OnDialogEnded; // Event to signal when the dialogue ends

        private Dictionary<string, Action> externalFunctions = new Dictionary<string, Action>();

        private void Update()
        {
            HandleSentenceSkipping();
        }

        public void StartDialog(DialogNodeGraph dialogNodeGraph)
        {
            isDialogStarted = true;

            if (dialogNodeGraph.nodesList == null)
            {
                Debug.LogWarning("Dialog Graph's node list is empty");
                return;
            }

            onDialogStarted?.Invoke();

            currentNodeGraph = dialogNodeGraph;

            DefineFirstNode(dialogNodeGraph);
            HandleDialogGraphCurrentNode(currentNode);
        }

        public void BindExternalFunction(string funcName, Action function)
        {
            externalFunctions[funcName] = function;
        }

        public void CallExternalFunction(string funcName)
        {
            if (externalFunctions.ContainsKey(funcName))
            {
                externalFunctions[funcName]?.Invoke();
            }
            else
            {
                Debug.LogWarning("External function '" + funcName + "' not found.");
            }
        }

        public bool ExternalFunctionExists(string funcName)
        {
            return externalFunctions.ContainsKey(funcName);
        }

        private void DefineFirstNode(DialogNodeGraph dialogNodeGraph)
        {
            if (dialogNodeGraph.nodesList.Count == 0)
            {
                Debug.LogWarning("The list of nodes in the DialogNodeGraph is empty");
                return;
            }

            foreach (Node node in dialogNodeGraph.nodesList)
            {
                currentNode = node;

                if (node.GetType() == typeof(SentenceNode))
                {
                    SentenceNode sentenceNode = (SentenceNode)node;

                    if (sentenceNode.parentNode == null && sentenceNode.childNode != null)
                    {
                        currentNode = sentenceNode;
                        return;
                    }
                }
            }

            currentNode = dialogNodeGraph.nodesList[0];
        }

        private void HandleDialogGraphCurrentNode(Node currentNode)
        {
            if (currentNode.GetType() == typeof(SentenceNode))
            {
                HandleSentenceNode(currentNode);
            }
        }

        private void HandleSentenceNode(Node currentNode)
        {
            SentenceNode sentenceNode = (SentenceNode)currentNode;

            isCurrentSentenceSkipped = false;

            OnSentenceNodeActive?.Invoke(sentenceNode.GetSentenceText());

            // No automatic progression, rely on SentenceProgression script
        }


        private System.Collections.IEnumerator WriteDialogTextRoutine(string text)
        {
            foreach (char textChar in text)
            {
                if (isCurrentSentenceSkipped)
                {
                    OnDialogTextSkipped?.Invoke(text);
                    break;
                }

                yield return new WaitForSeconds(dialogCharDelay);
            }

            CheckForDialogNextNode();
        }

        private void CheckForDialogNextNode()
        {
            if (currentNode.GetType() == typeof(SentenceNode))
            {
                SentenceNode sentenceNode = (SentenceNode)currentNode;

                if (sentenceNode.childNode != null)
                {
                    currentNode = sentenceNode.childNode;
                    HandleDialogGraphCurrentNode(currentNode);
                }
                else
                {
                    isDialogStarted = false;
                    onDialogFinished?.Invoke();

                    // Signal that the dialogue has ended
                    OnDialogEnded?.Invoke();
                }
            }
        }
        public void ProgressToNextSentenceNode() // Make the method public
        {
            if (!isDialogStarted)
            {
                Debug.LogWarning("Dialogue is not started.");
                return;
            }

            CheckForDialogNextNode();
        }

        private void HandleSentenceSkipping()
        {
            if (!isDialogStarted)
                return;

            // If you want to handle sentence skipping, put your logic here
        }
    }
}
