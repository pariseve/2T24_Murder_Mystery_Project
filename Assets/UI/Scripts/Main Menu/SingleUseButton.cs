using UnityEngine;
using UnityEngine.UI; // Import this if you are using Unity UI

public class SingleUseButton : MonoBehaviour
{
    [SerializeField] private Button button;
    private bool isButtonUsed = false;

    void Start()
    {
        OnEnable();
        // Add a listener to the button's onClick event
        button.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        // Check if the button has already been used
        if (!isButtonUsed)
        {
            // Execute your button click logic here
            Debug.Log("Button pressed!");

            // Set the flag to true so the button cannot be pressed again
            isButtonUsed = true;

            // Optionally disable the button to visually indicate it cannot be pressed
            button.interactable = false;
        }
    }

    // Optional: Reset button state on scene reload
    private void OnEnable()
    {
        // Reset the button state when the scene is loaded
        isButtonUsed = false;
        button.interactable = true;
    }
}
