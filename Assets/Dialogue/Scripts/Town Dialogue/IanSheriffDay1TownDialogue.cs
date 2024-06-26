using UnityEngine;
using TMPro;
using DialogueEditor;

public class IanSheriffDay1TownDialogue : MonoBehaviour
{
    [SerializeField] private NPCConversation dialogue;
    [SerializeField] private bool isInsideTrigger;
    [SerializeField] private GameObject interactTextPrefab; // Reference to the TextMeshPro prefab
    private GameObject interactTextInstance; // Instance of the prefab
    private KeyCode startConversationKey = KeyCode.E;

    private const string CONVERSATION_TRIGGERED_KEY = "IanSheriffDay1TownConversationTriggered";

    private void Start()
    {
        // Load the key from PlayerPrefs if it exists
        if (PlayerPrefs.HasKey(CONVERSATION_TRIGGERED_KEY))
        {
            string keyName = PlayerPrefs.GetString(CONVERSATION_TRIGGERED_KEY);
            startConversationKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideTrigger = true;
            ShowInteractText(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideTrigger = false;
            HideInteractText();
        }
    }

    private void StartConversation()
    {
        // Disable player movement or any other setup needed before dialogue
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.DisableMovement();
        }

        // Start the conversation
        ConversationManager.Instance.StartConversation(dialogue);

        // Save the key to PlayerPrefs
        PlayerPrefs.SetString(CONVERSATION_TRIGGERED_KEY, startConversationKey.ToString());
        PlayerPrefs.Save();
    }

    private void Update()
    {
        // Check if the conversation has not started yet
        if (isInsideTrigger && !PlayerPrefs.HasKey(CONVERSATION_TRIGGERED_KEY) && Input.GetKeyDown(startConversationKey))
        {
            StartConversation();
        }
    }

    private void ShowInteractText(Transform playerTransform)
    {
        if (interactTextPrefab != null)
        {
            interactTextInstance = Instantiate(interactTextPrefab, playerTransform);
            interactTextInstance.transform.localPosition = new Vector3(0, 1, 0); // Adjust the position above the player's head
            TextMeshProUGUI textComponent = interactTextInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = "[E] to Interact";
            }
        }
    }

    private void HideInteractText()
    {
        if (interactTextInstance != null)
        {
            Destroy(interactTextInstance);
        }
    }

    // You can call this method if you ever need to reset the PlayerPrefs key (for testing purposes, etc.)
    public void ResetConversationTrigger()
    {
        PlayerPrefs.DeleteKey(CONVERSATION_TRIGGERED_KEY);
        PlayerPrefs.Save();
    }
}


