using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForTakeMedicationUI : MonoBehaviour
{
public void ForNeedingMedication()
    {
        if (TakeMedicationUI.Instance != null)
        {
            TakeMedicationUI.Instance.NeedsTheMedication();
        }
        else
        {
            Debug.LogError("Medication UI is null");
        }
    }
}
