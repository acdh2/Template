using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Camera & Rotation")]
    public Camera playerCamera;
    public float mouseSensitivity = 100f;
    public float xRotationMultiplier = 1f;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private float xRotation = 0f;
    private bool grounded;

    [Header("Stamina & Running")]
    private float stamina = 1f;
    private float maxStamina = 1f;
    private bool shouldRun = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleStamina();
        HandleRotation();
        HandleJump();
    }

    void FixedUpdate()
    {
        HandleWalking();
    }

    void HandleStamina()
    {
        if (stamina > 0f && Input.GetKeyDown(KeyCode.LeftShift))
            shouldRun = true;

        if (shouldRun)
        {
            stamina -= Time.deltaTime;
            if (stamina <= 0f)
            {
                stamina = 0f;
                shouldRun = false;
            }
        }
        else
        {
            stamina += Time.deltaTime;
            if (stamina > maxStamina)
                stamina = maxStamina;
        }
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime * xRotationMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            grounded = false;
        }
    }

    void HandleWalking()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        float currentSpeed = shouldRun ? 2f : 1f;
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime * currentSpeed);
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
                grounded = true;
        }
    }
}