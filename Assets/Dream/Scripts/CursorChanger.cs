using UnityEngine;

public class CursorChanger : MonoBehaviour
{
    public Texture2D newCursorTexture; // Assign this in the inspector
    public LayerMask layerToDetect;

    private Vector2 hotSpot;
    private Vector2 defaultHotSpot = Vector2.zero;

    private void Start()
    {
        if (newCursorTexture != null)
        {
            hotSpot = new Vector2(newCursorTexture.width / 2, newCursorTexture.height / 2);
        }
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, layerToDetect))
        {
            // Change the cursor to the new texture when hovering over objects in the specified layer
            Cursor.SetCursor(newCursorTexture, hotSpot, CursorMode.Auto);
        }
        else
        {
            // Change the cursor back to the default texture when not hovering over specified layer objects
            Cursor.SetCursor(null, defaultHotSpot, CursorMode.Auto);
        }
    }
}
