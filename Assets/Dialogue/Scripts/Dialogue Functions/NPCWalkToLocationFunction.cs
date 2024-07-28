using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCWalkToLocationFunction : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Transform target;
    [SerializeField] private float stopDistance = 1f;
    private bool isMoving = false;
    private bool finalFlipX = false; // The final flipX state to face after reaching the target
    [SerializeField] private string boolName = "";
    [SerializeField] private SpriteRenderer spriteRenderer; // Reference the SpriteRenderer

    private void Start()
    {
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent is not assigned.");
        }
        if (target == null)
        {
            Debug.LogError("Target is not assigned.");
        }
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is not assigned.");
        }
    }

    // Method to start walking with initial flipX state true
    public void StartWalkingFlipXTrue(bool finalFlipX)
    {
        Debug.Log("StartWalkingFlipXTrue called with finalFlipXState: " + finalFlipX);
        StartWalking(true, finalFlipX);
    }

    // Method to start walking with initial flipX state false
    public void StartWalkingFlipXFalse(bool finalFlipX)
    {
        Debug.Log("StartWalkingFlipXFalse called with finalFlipXState: " + finalFlipX);
        StartWalking(false, finalFlipX);
    }

    private void StartWalking(bool initialFlipX, bool finalFlipX)
    {
        if (navMeshAgent != null && target != null)
        {
            Debug.Log("Start walking with initial flipX: " + initialFlipX + " and final flipX: " + finalFlipX);
            navMeshAgent.SetDestination(target.position);
            isMoving = true;
            ToggleFlipX(initialFlipX);
            this.finalFlipX = finalFlipX; // Set the final flipX state
            StartCoroutine(CheckArrival());
        }
        else
        {
            Debug.LogError("NavMeshAgent or Target is not assigned.");
        }
        if (boolName == null)
        {
            Debug.LogError("Bool name is not assigned.");
        }
    }

    private IEnumerator CheckArrival()
    {
        Debug.Log("CheckArrival started with finalFlipXState: " + finalFlipX);
        while (isMoving)
        {
            if (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                yield return null;
                continue;
            }

            if (Vector3.Distance(navMeshAgent.transform.position, target.position) <= stopDistance)
            {
                navMeshAgent.isStopped = true;
                isMoving = false;
                ToggleFlipX(finalFlipX); // Set the final flipX state
                if (boolName != null)
                {
                    SetBoolVariable();
                }
                yield break; // Exit the coroutine when the NPC reaches the destination
            }
            yield return null;
        }
    }

    public void ToggleFlipX(bool flipXState)
    {
        Debug.Log("ToggleFlipX called with flipXState: " + flipXState);
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = flipXState; // Toggle flipX state based on input
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isMoving)
        {
            navMeshAgent.isStopped = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isMoving)
        {
            navMeshAgent.isStopped = true;
        }
    }

    private void SetBoolVariable()
    {
        if (BoolManager.Instance != null)
        {
            BoolManager.Instance.SetBool(boolName, true);
        }
        else
        {
            Debug.LogError("BoolManager.Instance is null.");
        }
    }
}

