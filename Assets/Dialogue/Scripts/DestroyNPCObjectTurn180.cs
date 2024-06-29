using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class DestroyNPCObjectTurn180 : MonoBehaviour
{
    [SerializeField] private string npcObjectNameToMonitor;
    private NavMeshAgent navMeshAgent;
    [SerializeField] private Transform targetPosition;

    void Start()
    {
        ObjectManager.Instance.InitializeObjectKeys();
        Debug.Log($"Checking destruction status for NPC: {npcObjectNameToMonitor}");

        // Check if this NPC should be destroyed based on saved state
        if (ObjectManager.Instance.IsObjectDestroyed(npcObjectNameToMonitor))
        {
            Debug.Log($"Destroying NPC: {npcObjectNameToMonitor} because it was marked as destroyed.");
            Destroy(gameObject);
        }
    }

    public void StartDestroyNPC()
    {
        // Mark NPC as destroyed
        ObjectManager.Instance.MarkObjectDestroyed(npcObjectNameToMonitor);
        Debug.Log($"NPC: {npcObjectNameToMonitor} marked as destroyed.");

        // Initialize NavMeshAgent if not already
        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        // Move NPC towards the target position
        if (navMeshAgent != null && targetPosition != null)
        {
            navMeshAgent.enabled = true;
            navMeshAgent.autoBraking = false;
            navMeshAgent.SetDestination(targetPosition.position);
            Debug.Log($"Set destination to {targetPosition.position}");

            StartCoroutine(RotateTowardsDestination());
            StartCoroutine(DelayedCoroutine(8f));
            // StartCoroutine(WaitForNPCDestination(npcObjectNameToMonitor));
        }
        else
        {
            Debug.LogWarning("NavMeshAgent or targetPosition is not properly assigned.");
        }
    }

    private IEnumerator DelayedCoroutine(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        // Start the WaitForNPCDestination coroutine after the delay
        StartCoroutine(WaitForNPCDestination(npcObjectNameToMonitor));
    }

    private IEnumerator RotateTowardsDestination()
    {
        while (navMeshAgent.enabled && navMeshAgent.pathPending)
        {
            // Set the rotation to face a specific Y rotation (180 degrees in this case)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            yield return null;
        }
    }

    private IEnumerator WaitForNPCDestination(string npcObjectName)
    {
        GameObject npc = GameObject.Find(npcObjectName);

        if (npc != null && navMeshAgent != null && targetPosition != null)
        {
            // Wait until NPC reaches the target position
            while (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                yield return null;
            }

            Debug.Log($"NPC '{npcObjectName}' reached destination.");

            // Fade out NPC over 1 second
            Renderer renderer = npc.GetComponent<Renderer>();
            if (renderer != null)
            {
                float elapsedTime = 0f;
                Color startColor = renderer.material.color;
                Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // Fully transparent

                while (elapsedTime < 1f)
                {
                    elapsedTime += Time.deltaTime;
                    renderer.material.color = Color.Lerp(startColor, endColor, elapsedTime / 1f);
                    yield return null;
                }
            }

            // Destroy the NPC game object
            Destroy(npc);
        }
        else
        {
            Debug.LogWarning($"NPC '{npcObjectName}' not found or NavMesh components not properly assigned.");
        }
    }
}