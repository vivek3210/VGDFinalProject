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

    public float MoveAmount { get; private set; }
    public float TurnInput { get; private set; }
    public float ForwardInput { get; private set; }
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
        float moveZ = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");

        ForwardInput = moveZ;
        TurnInput = turn;
        MoveAmount = Mathf.Abs(moveZ);
        IsGrounded = controller.isGrounded;

        transform.Rotate(0f, turn * rotationSpeed * Time.deltaTime, 0f);

        Vector3 moveDir = transform.forward * moveZ;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
            isSprinting = false;
            controller.Move(moveDir * crouchSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = true;
            isCrouching = false;
            controller.Move(moveDir * sprintSpeed * Time.deltaTime);
        }
        else
        {
            isCrouching = false;
            isSprinting = false;
            controller.Move(moveDir * walkSpeed * Time.deltaTime);
        }

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

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