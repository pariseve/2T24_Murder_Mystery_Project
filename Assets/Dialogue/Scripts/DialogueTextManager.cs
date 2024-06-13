using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTextManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI DialogueText;

    // Method to change the text color of DialogueText
    public void SetTextColor(string colorHex)
    {
        if (DialogueText != null)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(colorHex, out color))
            {
                DialogueText.color = color;
            }
            else
            {
                Debug.LogWarning("Invalid color format: " + colorHex);
            }
        }
        else
        {
            Debug.LogWarning("DialogueText reference is not set. Assign the DialogueText reference in the inspector.");
        }
    }
}
