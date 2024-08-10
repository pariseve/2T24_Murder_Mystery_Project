using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InputChecker : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField inputField; // Reference to the InputField
    public TextMeshProUGUI resultText; // Reference to the TextMeshPro text element
    public Button checkButton; // Reference to the button

    [Header("Correct Answer")]
    [SerializeField] private string correctAnswer = "YourAnswer"; // The correct answer

    void Start()
    {
        // Ensure the button's onClick event is set to call the CheckAnswer method
        checkButton.onClick.AddListener(CheckAnswer);
    }

    private void CheckAnswer()
    {
        // Get the text from the InputField and compare it to the correct answer
        if (inputField.text.Equals(correctAnswer, System.StringComparison.OrdinalIgnoreCase))
        {
            // If correct, display "Correct"
            resultText.text = "Correct";
            resultText.color = Color.green; // Optionally change color to green
        }
        else
        {
            // If incorrect, display "Incorrect"
            resultText.text = "Incorrect";
            resultText.color = Color.red; // Optionally change color to red
        }
    }
}