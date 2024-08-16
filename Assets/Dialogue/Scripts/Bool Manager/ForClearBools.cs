using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForClearBools : MonoBehaviour
{
    public void ForClearingBools()
    {
        if (BoolManager.Instance != null)
        {
            BoolManager.Instance.ClearAllBools();
        }
        else
        {
            Debug.LogError("BoolManager.Instance is null.");
        }
    }
}
