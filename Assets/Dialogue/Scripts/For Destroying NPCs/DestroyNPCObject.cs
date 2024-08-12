using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class DestroyNPCObject : MonoBehaviour
{
    [SerializeField] private string npcObjectNameToMonitor;
    private NavMeshAgent navMeshAgent;
    [SerializeField] private Transform targetPosition;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private string boolName = "";

    private Vector3 previousPosition;

    void Start()
    {
        ObjectManager.Instance.InitializeObjectKeys();
        Debug.Log($"Checking destruction status for NPC: {npcObjectNameToMonitor}");

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is not assigned.");
        }

        if (ObjectManager.Instance.IsObjectDestroyed(npcObjectNameToMonitor))
        {
            Debug.Log($"Destroying NPC: {npcObjectNameToMonitor} because it was marked as destroyed.");
            Destroy(gameObject);
        }

        if (boolName == null)
        {
            Debug.LogError("Bool name is not assigned.");
        }

        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        // Initialize previousPosition with the starting position of the NPC
        previousPosition = navMeshAgent.transform.position;
    }

    public void StartDestroyNPCFlipXTrue(bool finalFlipXState)
    {
        StartDestroyNPC(true, finalFlipXState);
    }

    public void StartDestroyNPCFlipXFalse(bool finalFlipXState)
    {
        StartDestroyNPC(false, finalFlipXState);
    }

    private void StartDestroyNPC(bool initialFlipXState, bool finalFlipXState)
    {
        ObjectManager.Instance.MarkObjectDestroyed(npcObjectNameToMonitor);
        Debug.Log($"NPC: {npcObjectNameToMonitor} marked as destroyed.");

        if (navMeshAgent != null && targetPosition != null)
        {
            navMeshAgent.enabled = true;
            navMeshAgent.autoBraking = false;
            navMeshAgent.SetDestination(targetPosition.position);
            Debug.Log($"Set destination to {targetPosition.position}");

            ToggleFlipX(initialFlipXState);
            StartCoroutine(CheckArrival(finalFlipXState));
            StartCoroutine(DelayedCoroutine(8f));
        }
        else
        {
            Debug.LogWarning("NavMeshAgent or targetPosition is not properly assigned.");
        }
    }

    private IEnumerator CheckArrival(bool finalFlipXState)
    {
        while (navMeshAgent.enabled && navMeshAgent.pathPending)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            yield return null;
        }

        while (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            UpdateFlipX();  // Update flip state based on movement direction
            yield return null;
        }

        // NPC has reached the destination
        navMeshAgent.isStopped = true;
        ToggleFlipX(finalFlipXState);

        if (boolName != null)
        {
            SetBoolVariable();
        }
    }

    private void UpdateFlipX()
    {
        Vector3 currentPosition = navMeshAgent.transform.position;
        if (currentPosition.x > previousPosition.x)
        {
            spriteRenderer.flipX = false; // Moving right
        }
        else if (currentPosition.x < previousPosition.x)
        {
            spriteRenderer.flipX = true; // Moving left
        }
        previousPosition = currentPosition;
    }

    private IEnumerator DelayedCoroutine(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        StartCoroutine(WaitForNPCDestination(npcObjectNameToMonitor));
    }

    private IEnumerator WaitForNPCDestination(string npcObjectName)
    {
        GameObject npc = GameObject.Find(npcObjectName);

        if (npc != null && navMeshAgent != null && targetPosition != null)
        {
            while (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                yield return null;
            }

            Debug.Log($"NPC '{npcObjectName}' reached destination.");

            Renderer renderer = npc.GetComponent<Renderer>();
            if (renderer != null)
            {
                float elapsedTime = 0f;
                Color startColor = renderer.material.color;
                Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

                while (elapsedTime < 1f)
                {
                    elapsedTime += Time.deltaTime;
                    renderer.material.color = Color.Lerp(startColor, endColor, elapsedTime / 1f);
                    yield return null;
                }
            }

            Destroy(npc);

            if (boolName != null)
            {
                SetBoolVariable();
            }
        }
        else
        {
            Debug.LogWarning($"NPC '{npcObjectName}' not found or NavMesh components not properly assigned.");
        }
    }

    public void ToggleFlipX(bool flipXState)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = flipXState;
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

