using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3.5f;
    public float crouchSpeed = 2f;
    public float sprintSpeed = 6f;
    public float rotationSpeed = 120f;
    public float gravity = -9.81f;

    [Header("Stealth Settings")]
    public bool isCrouching;
    public bool isSprinting;
    [Range(0f, 1f)] public float visibility = 0.5f;
    [Range(0f, 1f)] public float noiseLevel = 0.2f;

    // Animator-readable values
    public float ForwardInput { get; private set; }
    public float TurnInput { get; private set; }
    public float MoveAmount { get; private set; }
    public bool IsGrounded { get; private set; }

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovement();
        HandleStealthValues();
    }

    void HandleMovement()
    {
        // Input
        float moveZ = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");

        // Save values for animator
        ForwardInput = moveZ;
        TurnInput = turn;
        MoveAmount = Mathf.Clamp01(Mathf.Abs(moveZ));

        // Rotation
        transform.Rotate(0f, turn * rotationSpeed * Time.deltaTime, 0f);

        // Grounded check
        IsGrounded = controller.isGrounded;

        if (IsGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        // Determine direction and speed
        Vector3 moveDir = transform.forward * moveZ;
        float currentSpeed = walkSpeed;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
            isSprinting = false;
            currentSpeed = crouchSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = true;
            isCrouching = false;
            currentSpeed = sprintSpeed;
        }
        else
        {
            isCrouching = false;
            isSprinting = false;
            currentSpeed = walkSpeed;
        }

        // Move player
        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleStealthValues()
    {
        if (isSprinting)
        {
            visibility = 1f;
            noiseLevel = 1f;
        }
        else if (isCrouching)
        {
            visibility = 0.25f;
            noiseLevel = 0.1f;
        }
        else
        {
            visibility = 0.6f;
            noiseLevel = 0.5f;
        }
    }
}