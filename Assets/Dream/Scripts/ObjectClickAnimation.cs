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

    private NPCConversation npcConversation;

    private void Start()
    {
        npcConversation = FindObjectOfType<NPCConversation>();

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
        // Trigger animation on all children
        foreach (Transform child in obj)
        {
            Animator animator = child.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger(animationTriggerName);
            }
        }

        // Wait for the fixed delay
        yield return new WaitForSeconds(animationDelay);

        // Start moving up and spreading out
        StartCoroutine(MoveUpAndSpreadChildren(obj));
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



