using UnityEngine;
using System.Collections;
using DialogueEditor;

public class ObjectClickAnimation : MonoBehaviour
{
    [SerializeField] private float moveDistance = 20f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float spreadDistance = 5f;
    [SerializeField] private float spreadDuration = 1.0f;
    [SerializeField] private string animationTriggerName = "PlayAnimation"; // The trigger name in the Animator
    [SerializeField] private float animationDelay = 0.5f; // Delay before moving up

    //for new sprite implementation
    [SerializeField] private Sprite stationarySprite;
    [SerializeField] private Sprite flyingSprite;
    //---------------------------

    [SerializeField] private KeyCode startKey = KeyCode.Mouse0;

    private NPCConversation npcConversation;

    private void Start()
    {
        npcConversation = FindObjectOfType<NPCConversation>();

    }

    void Update()
    {
        if (Input.GetKeyDown(startKey))
        {
            CheckClick();
        }
    }

    void CheckClick()
    {
        // Perform a raycast from the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("Crow Fly Click"))// && !npcConversation.isDialogueActive)
            {
                    StartCoroutine(PlayAnimationAndMoveUp(hit.transform));
            }
        }
    }

    private IEnumerator PlayAnimationAndMoveUp(Transform obj)
    {
        // Trigger animation on all nested children
        TriggerAnimationOnAllChildren(obj);

        // Wait for the fixed delay
        yield return new WaitForSeconds(animationDelay);

        // Start moving up and spreading out
        StartCoroutine(MoveUpAndSpreadChildren(obj));
    }

    private void TriggerAnimationOnAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Animator animator = child.GetComponent<Animator>();
            SpriteRenderer renderer = child.GetComponent<SpriteRenderer>();
            if (animator != null)
            {
                animator.SetTrigger(animationTriggerName);
                Debug.Log("has triggered animation");
            }
            else if (renderer != null)
            {
                Debug.Log("Changing sprite for " + child.name);
                renderer.sprite = flyingSprite;
            }
            else
            {
                Debug.Log("No SpriteRenderer found on " + child.name);
            }
            // Recursively apply animation to all children of this child
            TriggerAnimationOnAllChildren(child);
        }
    }


    private IEnumerator MoveUpAndSpreadChildren(Transform obj)
    {
        Vector3 startPos = obj.position;
        Vector3 endPos = startPos + Vector3.up * moveDistance;

        float elapsedTime = 0f;
        float duration = moveDistance / moveSpeed;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Move the parent object up
            obj.position = Vector3.Lerp(startPos, endPos, t);

            // Spread out children simultaneously
            foreach (Transform child in obj)
            {
                Vector3 spreadDirection = (child.position - obj.position).normalized;
                Vector3 targetPosition = child.position + spreadDirection * spreadDistance;
                child.position = Vector3.Lerp(child.position, targetPosition, t / spreadDuration);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.position = endPos;

        // Ensure children reach their final positions
        foreach (Transform child in obj)
        {
            Vector3 spreadDirection = (child.position - obj.position).normalized;
            Vector3 targetPosition = child.position + spreadDirection * spreadDistance;
            child.position = targetPosition;
        }

        // Destroy the parent object
        // Destroy(obj.gameObject);
    }

    private void DisableAllColliders()
    {
        Collider[] allColliders = FindObjectsOfType<Collider>();
        foreach (Collider collider in allColliders)
        {
            collider.enabled = false;
        }
    }

    private void EnableAllColliders()
    {
        Collider[] allColliders = FindObjectsOfType<Collider>();
        foreach (Collider collider in allColliders)
        {
            collider.enabled = true;
        }
    }
}



