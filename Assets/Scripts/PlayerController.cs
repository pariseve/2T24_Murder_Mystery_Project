using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 8f;
    public float sprintSpeed = 12f;
    public float gravity = 20f; // Added gravity
    public float groundDistance = 0.1f;
    public float smoothingFactor = 10f;
    public LayerMask groundLayer;
    public Rigidbody rb;
    public SpriteRenderer sr;

    public Sprite defaultSprite;

    private bool isGrounded = false;
    private bool isSprinting = false;
    private bool isIndoors = false;
    public bool canMove = true; // Flag to control movement

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            MoveCharacter();
        }
    }

    void MoveCharacter()
    {
        RaycastHit hit;
        Vector3 castPos = transform.position + Vector3.up;

        // Check if the player is grounded
        isGrounded = Physics.Raycast(castPos, -Vector3.up, out hit, groundDistance, groundLayer);

        // Apply gravity
        if (!isGrounded)
        {
            rb.velocity += Vector3.down * gravity * Time.deltaTime;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float y = isIndoors ? 0 : Input.GetAxisRaw("Vertical"); // Prevent z-axis movement indoors

        // Check if the player is sprinting
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Adjust speed based on sprinting state
        float currentSpeed = isSprinting ? sprintSpeed : speed;

        Vector3 moveDirection = new Vector3(x, 0, y).normalized * currentSpeed;

        // Apply movement directly to the Rigidbody for smoother physics
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

        // Handle character orientation (flipping sprite)
        if (x != 0)
        {
            sr.flipX = (x < 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Indoor"))
        {
            isIndoors = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Indoor"))
        {
            isIndoors = false;
        }
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void DisableMovement()
    {
        canMove = false;
        rb.velocity = Vector3.zero; // Stop the player immediately
    }

    public void InteriorMovement()
    {
        // Additional indoor movement logic if needed
    }
}

