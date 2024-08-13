using UnityEngine;

public class MirrorAnimationTrigger : MonoBehaviour
{
    [SerializeField] private CameraZoom cameraZoom;
    private Animator animator;

    public bool isFadingIn = false;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found in children.");
        }
    }

    private void Update()
    {
        TriggerAnim();
    }

    public void TriggerAnim()
    {
        if (cameraZoom.isZoomedInMirror && !isFadingIn)
        {
            Debug.Log("Fade in animation triggered");
            animator.SetBool("isFadingIn", true);
            animator.SetBool("isFadingOut", false);
            isFadingIn = true;
        }
        else if (!cameraZoom.isZoomedInMirror && isFadingIn)
        {
            Debug.Log("Fade out animation triggered");
            animator.SetBool("isFadingIn", false);
            animator.SetBool("isFadingOut", true);
            isFadingIn = false;
        }
    }
}
