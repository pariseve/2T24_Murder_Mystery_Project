using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForBoolManager : MonoBehaviour
{

    public void SetBoolVariable(string boolName)
    {
        if (BoolManager.Instance != null)
        {
            BoolManager.Instance.SetBool(boolName, true);
        }
        else
        {
            Debug.LogError("BoolManager.Instance is null.");
        }
    }

    public void GetBoolVariable(string boolName)
    {
        if (BoolManager.Instance != null)
        {
            bool value = BoolManager.Instance.GetBool(boolName);
            Debug.Log($"Boolean '{boolName}' value: {value}");
        }
        else
        {
            Debug.LogError("BoolManager.Instance is null.");
        }
    }
}
