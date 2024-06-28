using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearPlayerPrefs : MonoBehaviour
{
    private void Awake()
    {
        PlayerPrefs.DeleteAll();
    }

}
