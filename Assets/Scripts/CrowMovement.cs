using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class CrowMovement : MonoBehaviour
{
    [SerializeField] private string crowObject;
    [SerializeField] private Transform crow; // The crow object
    [SerializeField] private Transform targetLocation; // The target location the crow will move towards
    [SerializeField] private float flyUpHeight = 15f;
    [SerializeField] private float flyUpSpeed = 3f;
    [SerializeField] private string moveAnimationTriggerName = "MoveAnimation"; // The trigger name for moving animation
    [SerializeField] private string flyAnimationTriggerName = "FlyAnimation"; // The trigger name for flying animation
    [SerializeField] private string boolName = "";
    [SerializeField] private string moveAnimationBoolName = "IsMoving"; // The boolean parameter name in the Animator for the move animation

    private NavMeshAgent navMeshAgent;
    private Animator crowAnimator;
    private bool isMovingTowardsTarget = false;

    private void Start()
    {
        ObjectManager.Instance.InitializeObjectKeys();
        Debug.Log($"Checking destruction status for NPC: {crowObject}");

        // Check if this NPC should be destroyed based on saved state
        if (ObjectManager.Instance.IsObjectDestroyed(crowObject))
        {
            Debug.Log($"Destroying NPC: {crowObject} because it was marked as destroyed.");
            Destroy(gameObject);
        }
        if (crow != null)
        {
            navMeshAgent = crow.GetComponent<NavMeshAgent>();
            crowAnimator = crow.GetComponent<Animator>();

            if (!ObjectManager.Instance.IsObjectDestroyed(crowObject))
            {
                LoadCrowPosition();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveCrowPosition();
            isMovingTowardsTarget = true;
            if (crowAnimator != null)
            {
                crowAnimator.SetTrigger(moveAnimationTriggerName);
                crowAnimator.SetBool(moveAnimationBoolName, true);
            }

            if (navMeshAgent != null && targetLocation != null)
            {
                SetCrowRotation();
                navMeshAgent.SetDestination(targetLocation.position);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveCrowPosition();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isMovingTowardsTarget = false;
            if (navMeshAgent != null)
            {
                navMeshAgent.ResetPath();
            }
            if (crowAnimator != null)
            {
                crowAnimator.SetBool(moveAnimationBoolName, false);
            }
        }
    }

    private void Update()
    {
        if (isMovingTowardsTarget && navMeshAgent != null && targetLocation != null)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.pathPending)
            {
                isMovingTowardsTarget = false;
                StartCoroutine(FlyUpAndDestroy());
                ObjectManager.Instance.MarkObjectDestroyed(crowObject);
            }
        }
    }

    private void SetCrowRotation()
    {
        if (targetLocation != null)
        {
            Vector3 directionToTarget = targetLocation.position - crow.position;
            if (directionToTarget.x < 0) // Target is to the left
            {
                crow.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else // Target is to the right
            {
                crow.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
        }
    }

    private IEnumerator FlyUpAndDestroy()
    {
        if (crowAnimator != null)
        {
            crowAnimator.SetTrigger(flyAnimationTriggerName);
        }

        SetBoolVariable();

        // Disable the NavMeshAgent component before flying up
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }

        Vector3 targetPosition = new Vector3(crow.position.x, crow.position.y + flyUpHeight, crow.position.z);

        while (Vector3.Distance(crow.position, targetPosition) > 0.1f)
        {
            crow.position = Vector3.MoveTowards(crow.position, targetPosition, flyUpSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(crow.gameObject);
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

    private void SaveCrowPosition()
    {
        PlayerPrefs.SetFloat($"{crowObject}_PosX", crow.position.x);
        PlayerPrefs.SetFloat($"{crowObject}_PosY", crow.position.y);
        PlayerPrefs.SetFloat($"{crowObject}_PosZ", crow.position.z);
        PlayerPrefs.Save();
    }

    private void LoadCrowPosition()
    {
        if (PlayerPrefs.HasKey($"{crowObject}_PosX") && PlayerPrefs.HasKey($"{crowObject}_PosY") && PlayerPrefs.HasKey($"{crowObject}_PosZ"))
        {
            if (!ObjectManager.Instance.IsObjectDestroyed(crowObject))
            {
                float x = PlayerPrefs.GetFloat($"{crowObject}_PosX");
                float y = PlayerPrefs.GetFloat($"{crowObject}_PosY");
                float z = PlayerPrefs.GetFloat($"{crowObject}_PosZ");
                crow.position = new Vector3(x, y, z);
            }
            else
            {
                ObjectManager.Instance.MarkObjectDestroyed(crowObject);
            }
        }
    }
}