using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MessageManager))]
public class MessageManagerEditor : Editor
{
    private string parameters = ""; // Field to hold the input parameters

    public override void OnInspectorGUI()
    {
        MessageManager messageManager = (MessageManager)target;

        DrawDefaultInspector();

        // Add a text field to input the parameters
        parameters = EditorGUILayout.TextField("Message Parameters", parameters);

        if (GUILayout.Button("Trigger Messages"))
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                messageManager.ForCreateMultipleNPCMessages(parameters);
            }
            else
            {
                Debug.LogError("Parameters cannot be empty.");
            }
        }
    }
}
