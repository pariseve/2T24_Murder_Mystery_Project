using UnityEngine;

public class TestAnimatorTrigger : MonoBehaviour
{
    [SerializeField] private string animationTriggerName = "PlayAnimation";
    [SerializeField] private string isMovingBoolName = "IsMoving";

    private void Start()
    {
        TriggerAnimationOnAllChildren(transform);
    }

    private void TriggerAnimationOnAllChildren(Transform parent)
    {
        Debug.Log($"TriggerAnimationOnAllChildren called for parent: {parent.name}");

        foreach (Transform child in parent)
        {
            Debug.Log($"Processing child: {child.name}");

            Animator animator = child.GetComponent<Animator>();
            if (animator != null)
            {
                Debug.Log($"Animator found on {child.name}");

                animator.SetTrigger(animationTriggerName);
                animator.SetBool(isMovingBoolName, true);

                Debug.Log($"Trigger {animationTriggerName} set on {child.name}");
                Debug.Log($"IsMoving set to true on {child.name}");
            }
            else
            {
                Debug.LogWarning($"No Animator found on {child.name}");
            }

            TriggerAnimationOnAllChildren(child);
        }
    }
}

