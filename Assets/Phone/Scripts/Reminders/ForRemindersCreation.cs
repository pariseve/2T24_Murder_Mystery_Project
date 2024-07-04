using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForRemindersCreation : MonoBehaviour
{
    public void ForInstantiateReminders()
    {
        if (RemindersManager.Instance != null)
        {
            RemindersManager.Instance.InstantiateReminders();
        }
        else
        {
            Debug.LogError("NotesManager.Instance is null.");
        }
    }
}

