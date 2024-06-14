using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForCreatingContactAndMessage : MonoBehaviour
{
    public void NPCMessage(string parameters)
    {
        if (MessageManager.Instance != null)
        {
            // Assuming "parameters" is correctly formatted for ForCreateNPCMessage
            MessageManager.Instance.ForCreateNPCMessage(parameters);
        }
        else
        {
            Debug.LogError("MessageManager.Instance is null. Ensure MessageManager is set up correctly.");
        }
    }

    public void ForContact(string npcName)
    {
        if (MessageManager.Instance != null)
        {
            MessageManager.Instance.ForInstantiateContact(npcName);
        }
        else
        {
            Debug.LogError("MessageManager.Instance is null. Ensure MessageManager is set up correctly.");
        }
    }
}

