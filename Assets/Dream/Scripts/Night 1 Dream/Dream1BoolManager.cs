using UnityEngine;
using System.Reflection;

public class Dream1BoolManager : MonoBehaviour
{
    public bool brokenClock;
    public bool mirror;

    public void SetBool(string boolName, bool value)
    {
        FieldInfo field = GetType().GetField(boolName, BindingFlags.Public | BindingFlags.Instance);

        if (field != null && field.FieldType == typeof(bool))
        {
            field.SetValue(this, value);
            Debug.Log($"Set {boolName} to {value}");
        }
        else
        {
            Debug.LogError($"Bool with name {boolName} does not exist.");
        }
    }

    // Intermediary methods to be called by the button's OnClick event
    public void SetBrokenClockTrue()
    {
        SetBool("brokenClock", true);
    }

    public void SetMirrorTrue()
    {
        SetBool("mirror", true);
    }
}


