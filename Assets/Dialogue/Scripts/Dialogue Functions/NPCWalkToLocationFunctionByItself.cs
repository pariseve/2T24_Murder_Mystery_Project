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
    
    private Vector3 previousPosition;

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

        // Initialize previousPosition with the starting position of the NPC
        previousPosition = navMeshAgent.transform.position;

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

    public void StartWalkingFlipXTrue(bool finalFlipXState)
    {
        StartWalking(true, finalFlipXState);
    }

    public void StartWalkingFlipXFalse(bool finalFlipXState)
    {
        StartWalking(false, finalFlipXState);
    }

    private void StartWalking(bool initialFlipXState, bool finalFlipXState)
    {
        if (navMeshAgent != null && target != null)
        {
            if (NavMesh.SamplePosition(target.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
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
        while (isMoving)
        {
            if (navMeshAgent.pathPending)
            {
                yield return null;
                continue;
            }

            // Update the flipX state dynamically based on movement direction
            UpdateFlipX();

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

    public void ToggleFlipX(bool flipXState)
    {
        spriteRenderer.flipX = flipXState;
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
