using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject playCanvas;   // Reference to the Play Canvas
    [SerializeField] private GameObject optionsCanvas; // Reference to the Options Canvas

    // Start is called before the first frame update
    private void Start()
    {
        // Make sure both canvases are inactive at the start
        playCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
    }

    // Method to show the Play Canvas
    public void ShowPlayCanvas()
    {
        playCanvas.SetActive(true);
        optionsCanvas.SetActive(false);
    }

    // Method to hide the Play Canvas
    public void HidePlayCanvas()
    {
        playCanvas.SetActive(false);
    }

    // Method to show the Options Canvas
    public void ShowOptionsCanvas()
    {
        optionsCanvas.SetActive(true);
        playCanvas.SetActive(false);
    }

    // Method to hide the Options Canvas
    public void HideOptionsCanvas()
    {
        optionsCanvas.SetActive(false);
    }
}

