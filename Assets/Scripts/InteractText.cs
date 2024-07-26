using UnityEngine;
using TMPro;
using System.Collections;

public class InteractText : MonoBehaviour
{
    private GameObject playerCharacter;
    private TextMeshPro textMeshPro;
    [TextArea(10, 15)]
    public string interactPrompt;

    void Start()
    {
        StartCoroutine(CheckAgain());
    }

    public void SetText(string text)
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = text;
        }
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

    IEnumerator CheckAgain()
    {
        while (playerCharacter == null)
        {
            playerCharacter = GameObject.FindGameObjectWithTag("Player");
            yield return new WaitForSeconds(0.5f);
        }

        textMeshPro = playerCharacter.GetComponentInChildren<TextMeshPro>();
        if (textMeshPro == null)
        {
            Debug.LogError("TextMeshPro component not found in children of player character.");
        }
    }
}
