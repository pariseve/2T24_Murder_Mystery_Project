using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerClickDialogue : MonoBehaviour
{
    [SerializeField] private float moveDistance = 20f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float spreadDistance = 5f;
    [SerializeField] private float spreadDuration = 1.0f;
    [SerializeField] private string animationTriggerName = "PlayAnimation"; // The trigger name in the Animator
    [SerializeField] private float animationDelay = 0.5f; // Delay before moving up
    [SerializeField] private string isMovingBoolName = "IsMoving"; // The bool name in the Animator
    [SerializeField] private KeyCode startKey = KeyCode.E; // Key to press for activation

    private bool isPlayerInTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
        }
    }

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(startKey))
        {
            CheckClick();
        }
    }

    void CheckClick()
    {
                StartCoroutine(PlayAnimationAndMoveUp(transform));
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
            if (animator != null)
            {
                animator.SetTrigger(animationTriggerName);
                animator.SetBool(isMovingBoolName, true); // Set the IsMoving bool to true
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
        Destroy(obj.gameObject);
    }
}