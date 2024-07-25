using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;

    [SerializeField] private RectTransform targetRectTransform; // Use RectTransform for UI elements
    [SerializeField] private Canvas canvas; // Reference to the Canvas that this UI element is a part of

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>(); // Find the parent Canvas if not set
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        Vector2 localPoint;
        // Convert mouse position to local position in the canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform, // Assuming the parent is the canvas RectTransform
            eventData.position,
            canvas.worldCamera,
            out localPoint);

        transform.GetComponent<RectTransform>().anchoredPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
    }

    public RectTransform GetTargetRectTransform()
    {
        return targetRectTransform;
    }
}



