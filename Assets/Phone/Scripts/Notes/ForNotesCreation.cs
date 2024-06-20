using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForNotesCreation : MonoBehaviour
{
    public void ForInstantiateNotes()
    {
        if (NotesManager.Instance != null)
        {
            NotesManager.Instance.InstantiateNotes();
        }
        else
        {
            Debug.LogError("NotesManager.Instance is null.");
        }
    }

    public void SetNoteBool(string noteName)
    {
        if (NotesManager.Instance != null)
        {
            NotesManager.Instance.SetNoteBool(noteName);
        }
        else
        {
            Debug.LogError("NotesManager.Instance is null.");
        }
    }
}
