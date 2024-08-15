using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CursorChanger : MonoBehaviour
{
    public Texture2D newCursorTexture; // Assign this in the inspector
    public LayerMask layerToDetect;

    private Vector2 hotSpot;
    private Vector2 defaultHotSpot = Vector2.zero;

    public static CursorChanger Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (newCursorTexture != null)
        {
            hotSpot = new Vector2(newCursorTexture.width / 2, newCursorTexture.height / 2);
        }
    }

    private void Update()
    {
        if (IsPointerOverUIElement())
        {
            // If the pointer is over a UI element, reset the cursor to the default
            Cursor.SetCursor(null, defaultHotSpot, CursorMode.Auto);
            return;
        }

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

    private bool IsPointerOverUIElement()
    {
        // Get the current event system
        EventSystem eventSystem = EventSystem.current;

        // Get all the EventSystems in the scene, including those in DontDestroyOnLoad objects
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();

        // Loop through each EventSystem to check if the pointer is over any UI elements
        foreach (var es in eventSystems)
        {
            if (es.IsPointerOverGameObject())
            {
                // Check if the pointer is over a UI element
                PointerEventData eventData = new PointerEventData(es)
                {
                    position = Input.mousePosition
                };

                List<RaycastResult> results = new List<RaycastResult>();
                es.RaycastAll(eventData, results);

                foreach (var result in results)
                {
                    // Check if the UI element is in front of the collider
                    if (result.gameObject != null && result.gameObject.GetComponent<CanvasRenderer>() != null)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
