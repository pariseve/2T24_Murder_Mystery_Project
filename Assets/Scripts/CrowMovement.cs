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
    [SerializeField] private Sprite idleSprite; // The sprite when the crow is idle
    [SerializeField] private Sprite moveSprite; // The sprite when the crow is moving
    [SerializeField] private string boolName = "";

    private NavMeshAgent navMeshAgent;
    private SpriteRenderer spriteRenderer; // SpriteRenderer for changing sprites
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
            spriteRenderer = crow.GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component

            if (!ObjectManager.Instance.IsObjectDestroyed(crowObject))
            {
                LoadCrowPosition();
            }

            // Set the initial sprite to idle
            if (spriteRenderer != null && idleSprite != null)
            {
                spriteRenderer.sprite = idleSprite;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveCrowPosition();
            isMovingTowardsTarget = true;
            if (spriteRenderer != null && moveSprite != null)
            {
                spriteRenderer.sprite = moveSprite; // Change to move sprite
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

            if (spriteRenderer != null && idleSprite != null)
            {
                spriteRenderer.sprite = idleSprite; // Change back to idle sprite
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