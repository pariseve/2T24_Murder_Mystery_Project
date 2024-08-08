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
            Debug.LogError("ReminderManager.Instance is null.");
        }
    }

    public void ForRemovingReminder(string reminderName)
    {
        if (RemindersManager.Instance != null)
        {
            RemindersManager.Instance.RemoveReminderByName(reminderName);
        }
        else
        {
            Debug.LogError("ReminderManager.Instance is null.");
        }
    }

    public void ForSetReminderToIncomplete(string reminderName)
    {
        if (RemindersManager.Instance != null)
        {
            RemindersManager.Instance.SetDescriptionToIncomplete(reminderName);
        }
        else
        {
            Debug.LogError("ReminderManager.Instance is null.");
        }
    }
}

