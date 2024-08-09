using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCWalkToLocationFunctionByItself : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Transform target;
    [SerializeField] private float stopDistance = 1f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private bool isMoving = false;
    [SerializeField] private string boolName = "";

    private string GetNavMeshAgentID()
    {
        // Use the instance ID of the NavMeshAgent to generate a unique key
        return navMeshAgent.GetInstanceID().ToString();
    }

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
        if (boolName == null)
        {
            Debug.LogError("Bool name is not assigned.");
        }

        // Load the saved position when starting
        LoadTargetPosition();
    }

    private void OnDisable()
    {
        if (navMeshAgent != null)
        {
            SaveTargetPosition();
        }
    }

    // Method to start walking with initial flipX state true
    public void StartWalkingFlipXTrue(bool finalFlipXState)
    {
        Debug.Log("StartWalkingFlipXTrue called with finalFlipXState: " + finalFlipXState);
        if (spriteRenderer != null)
        {
            StartWalking(true, finalFlipXState);
        }
        else
        {
            Debug.LogError("SpriteRenderer is not assigned.");
        }
    }

    // Method to start walking with initial flipX state false
    public void StartWalkingFlipXFalse(bool finalFlipXState)
    {
        Debug.Log("StartWalkingFlipXFalse called with finalFlipXState: " + finalFlipXState);
        if (spriteRenderer != null)
        {
            StartWalking(false, finalFlipXState);
        }
        else
        {
            Debug.LogError("SpriteRenderer is not assigned.");
        }
    }

    private void StartWalking(bool initialFlipXState, bool finalFlipXState)
    {
        Debug.Log("StartWalking called with initialFlipXState: " + initialFlipXState + " and finalFlipXState: " + finalFlipXState);
        if (navMeshAgent != null && target != null)
        {
            if (NavMesh.SamplePosition(target.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                Debug.Log("Start walking");
                navMeshAgent.SetDestination(hit.position);
                isMoving = true;
                navMeshAgent.isStopped = false; // Ensure the agent is not stopped
                ToggleFlipX(initialFlipXState); // Set the initial flipX state
                StartCoroutine(CheckArrival(finalFlipXState));
            }
            else
            {
                Debug.LogError("Target position is not on the NavMesh.");
            }
        }
    }

    private IEnumerator CheckArrival(bool finalFlipXState)
    {
        Debug.Log("CheckArrival started with finalFlipXState: " + finalFlipXState);
        while (isMoving)
        {
            if (navMeshAgent.pathPending)
            {
                yield return null;
                continue;
            }

            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    navMeshAgent.isStopped = true;
                    isMoving = false;
                    ToggleFlipX(finalFlipXState); // Set the final flipX state
                    if (boolName != null)
                    {
                        SetBoolVariable();
                    }
                }
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

    private void SaveTargetPosition()
    {
        PlayerPrefs.SetFloat($"{navMeshAgent}_Pos_X", navMeshAgent.transform.position.x);
        PlayerPrefs.SetFloat($"{navMeshAgent}_Pos_Y", navMeshAgent.transform.position.y);
        PlayerPrefs.SetFloat($"{navMeshAgent}_Pos_Z", navMeshAgent.transform.position.z);
        PlayerPrefs.SetFloat($"{navMeshAgent}_FlipX", spriteRenderer.flipX ? 1f : 0f);
        PlayerPrefs.Save(); // Ensure the data is saved immediately
    }

    private void LoadTargetPosition()
    {
        if (PlayerPrefs.HasKey($"{navMeshAgent}_Pos_X") &&
            PlayerPrefs.HasKey($"{navMeshAgent}_Pos_Y") &&
            PlayerPrefs.HasKey($"{navMeshAgent}_Pos_Z") &&
            PlayerPrefs.HasKey($"{navMeshAgent}_FlipX"))
        {
            float x = PlayerPrefs.GetFloat($"{navMeshAgent}_Pos_X");
            float y = PlayerPrefs.GetFloat($"{navMeshAgent}_Pos_Y");
            float z = PlayerPrefs.GetFloat($"{navMeshAgent}_Pos_Z");
            bool flipX = PlayerPrefs.GetFloat($"{navMeshAgent}_FlipX") > 0f;
            navMeshAgent.transform.position = new Vector3(x, y, z);
            ToggleFlipX(flipX);
        }
    }
}