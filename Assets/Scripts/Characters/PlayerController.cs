using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3.5f;
    public float crouchSpeed = 2f;
    public float sprintSpeed = 6f;
    public float gravity = -9.81f;

    [Header("Stealth Settings")]
    public bool isCrouching;
    public bool isSprinting;

    [Range(0f, 1f)] public float visibility = 0.5f;  // 0 = invisible, 1 = fully visible
    [Range(0f, 1f)] public float noiseLevel = 0.2f;  // 0 = silent, 1 = loud

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
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
            isSprinting = false;
            controller.Move(move * crouchSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = true;
            isCrouching = false;
            controller.Move(move * sprintSpeed * Time.deltaTime);
        }
        else
        {
            isCrouching = false;
            isSprinting = false;
            controller.Move(move * walkSpeed * Time.deltaTime);
        }

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