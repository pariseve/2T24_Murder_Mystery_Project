using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FunctionTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent function;

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player") &&  Input.GetKeyDown(KeyCode.Space)) 
        {
            function.Invoke();
        }
    }

}
