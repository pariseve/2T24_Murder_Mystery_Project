using UnityEngine;
using UnityEngine.EventSystems;

public class IconClickHandler : MonoBehaviour, IPointerClickHandler
{
    public string iconName; // Name of the icon

    private PhoneOnboarding phoneOnboarding;

    void Start()
    {
        // Find the PhoneOnboarding instance
        phoneOnboarding = FindObjectOfType<PhoneOnboarding>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Trigger the onboarding step progress
        phoneOnboarding.OnIconClicked(iconName);
    }
}

