using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerController))]
public class PlayerAnimationController : MonoBehaviour
{
    private Animator anim;
    private PlayerController controller;

    [Header("Animation Smoothing")]
    public float dampTime = 0.1f;

    void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();

        if (anim == null)
        {
            Debug.LogError("Animator component not found.");
        }

        if (controller == null)
        {
            Debug.LogError("PlayerController component not found.");
        }
    }

    void Start()
    {
        anim.applyRootMotion = false;
    }

    void Update()
    {
        // Float parameters
        anim.SetFloat("velx", controller.TurnInput, dampTime, Time.deltaTime);
        anim.SetFloat("vely", controller.ForwardInput, dampTime, Time.deltaTime);
        anim.SetFloat("speed", controller.MoveAmount, dampTime, Time.deltaTime);

        // Bool parameters
        anim.SetBool("isRunning", controller.isSprinting);
        anim.SetBool("isCrouching", controller.isCrouching);
        anim.SetBool("isGrounded", controller.IsGrounded);
        anim.SetBool("isFalling", !controller.IsGrounded);
    }
}