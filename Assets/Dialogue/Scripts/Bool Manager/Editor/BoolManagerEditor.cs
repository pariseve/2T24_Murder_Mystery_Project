using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

[CustomEditor(typeof(BoolManager))]
public class BoolManagerEditor : Editor
{
    private string newBoolName = "";
    private bool isDirty = false; // Flag to track changes for runtime update
    private List<string> boolsToRemove = new List<string>(); // List to store bools marked for removal

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BoolManager boolManager = (BoolManager)target; // Cast target to BoolManager

        GUILayout.Space(10);
        GUILayout.Label("Manage Booleans", EditorStyles.boldLabel);

        List<string> boolKeys = boolManager.GetBoolKeys();

        // Display current bools
        if (boolKeys != null && boolKeys.Count > 0)
        {
            foreach (var boolName in boolKeys)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(boolName);
                bool newValue = EditorGUILayout.Toggle(boolManager.GetBool(boolName));

                // If value changes, update the bool in BoolManager
                if (newValue != boolManager.GetBool(boolName))
                {
                    boolManager.SetBool(boolName, newValue);
                    EditorUtility.SetDirty(boolManager); // Mark ScriptableObject as dirty
                    isDirty = true; // Set flag for runtime update
                }

                // Button to mark for removal
                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    boolsToRemove.Add(boolName); // Mark for removal
                    isDirty = true; // Set flag for runtime update
                }

                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.Label("No booleans found.");
        }

        // Remove bools marked for removal after iterating through all bools
        if (boolsToRemove.Count > 0)
        {
            foreach (string boolNameToRemove in boolsToRemove)
            {
                boolManager.RemoveBool(boolNameToRemove);
            }
            boolsToRemove.Clear(); // Clear the list after removal
            EditorUtility.SetDirty(boolManager); // Mark ScriptableObject as dirty
            isDirty = true; // Set flag for runtime update
        }

        GUILayout.Space(10);

        // Add new boolean
        GUILayout.Label("Add New Boolean", EditorStyles.boldLabel);
        newBoolName = EditorGUILayout.TextField("Boolean Name", newBoolName);
        if (GUILayout.Button("Add Boolean"))
        {
            if (!string.IsNullOrEmpty(newBoolName))
            {
                boolManager.SetBool(newBoolName, false);
                EditorUtility.SetDirty(boolManager); // Mark ScriptableObject as dirty
                newBoolName = "";
                isDirty = true; // Set flag for runtime update
            }
        }

        // Save changes to the boolManager
        if (GUI.changed)
        {
            EditorSceneManager.MarkSceneDirty(boolManager.gameObject.scene);
            EditorUtility.SetDirty(boolManager); // Mark ScriptableObject as dirty
            isDirty = true; // Set flag for runtime update
        }

        // Repaint the GUI if necessary
        if (isDirty)
        {
            Repaint();
            isDirty = false; // Reset flag after repaint
        }
    }

    void OnEnable()
    {
        // Subscribe to editor update for runtime GUI refresh
        EditorApplication.update += OnEditorUpdate;
    }

    void OnDisable()
    {
        // Unsubscribe from editor update
        EditorApplication.update -= OnEditorUpdate;
    }

    void OnEditorUpdate()
    {
        // Check if changes were made during runtime and repaint GUI
        if (isDirty)
        {
            Repaint();
            isDirty = false; // Reset flag after repaint
        }
    }
}

