using UnityEngine;
using System.Collections.Generic;

public class SpriteChanger : MonoBehaviour
{
    public List<string> requiredBoolNamesTrue;
    public Sprite trueSprite; 

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateSprite();
    }

    void Update()
    {
        UpdateSprite();
    }

    private bool AllRequiredBoolsTrue()
    {
        foreach (string boolName in requiredBoolNamesTrue)
        {
            if (!BoolManager.Instance.GetBool(boolName))
            {
                return false;
            }
        }
        return true;
    }

    private void UpdateSprite()
    {
        if (AllRequiredBoolsTrue())
        {
            spriteRenderer.sprite = trueSprite;
        }
    }
}

