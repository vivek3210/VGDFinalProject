using UnityEngine;

public class CatController : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 700f;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(horizontal, 0, vertical);

        if (move.magnitude > 0.1f)
        {
            // Rotate cat toward movement direction
            Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        controller.Move(move * speed * Time.deltaTime);
    }
}