using UnityEngine;
using TMPro;

public class InteractText : MonoBehaviour
{
    public GameObject playerCharacter;
    private TextMeshPro textMeshPro;
    [TextArea(10,15)]
    public string interactPrompt;

    void Start()
    {
        playerCharacter = GameObject.FindGameObjectWithTag("Player");
        if (playerCharacter == null)
        {
            Debug.Log("no player character found :(");
            return;
        }
        
        if (textMeshPro == null)
        {
            textMeshPro = playerCharacter.GetComponentInChildren<TextMeshPro>();
            return;
        }


    }

    public void SetText(string text)
    {
        textMeshPro.text = interactPrompt;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetText(interactPrompt);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (textMeshPro != null)
            {
                textMeshPro.text = "";
            }
        }
    }
}
