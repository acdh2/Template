using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Camera & Rotation")]
    public Camera playerCamera;
    public float mouseSensitivity = 100f;

    public float xRotationMultiplier = 10f;

    [Header("Movement")]
    public float moveSpeed = 5f;

    public float jumpForce = 5f;

    private Rigidbody rb;
    private float xRotation = 0f;
    private bool grounded;


    private float stamina = 1.0f;
    private float maxStamina = 1.0f;
    private bool shouldRun = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void HandleStamina() {
        if (stamina > 0f && Input.GetKeyDown(KeyCode.LeftShift))
        {
            shouldRun = true;
        }

        if (shouldRun) {
            stamina -= Time.deltaTime;
            if (stamina < 0f) {
                stamina = 0f;
                shouldRun = false;
            }
        } else {
            stamina += Time.deltaTime;
            if (stamina > maxStamina) stamina = maxStamina;
        }
    }

    void HandleRotation() {
        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime * xRotationMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleJump()
    {
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            grounded = false; // voorkom dubbel springen
        }
    }

    void HandleWalking()
    {
        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        float baseSpeed = 1.0f;
        if (shouldRun) baseSpeed = 2.0f;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime * baseSpeed);
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

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                grounded = true;
            }
        }
    }    
}

