using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractText : MonoBehaviour
{
    public GameObject textObj;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Has entered trigger");
        if (other.CompareTag("Player"))
        {
            textObj.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Has left trigger");
        if (other.CompareTag("Player"))
        {
            textObj.SetActive(false);
        }
    }
}
