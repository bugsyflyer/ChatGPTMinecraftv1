using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpHeight = 1.5f;
    public float walkSpeed = 2f;
    public float sprintSpeed = 4f;
    public int startingHealth = 100;
    public int fallDamageThreshold = 9;
    public GameObject sword;
    public GameObject pickaxe;

    private Rigidbody rb;
    private bool isGrounded = false;
    private bool isSprinting = false;
    private int currentHealth;
    
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    public float maxLookUpAngle = 90f;
    public float maxLookDownAngle = -90f;

    private float verticalRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = startingHealth;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Camera rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, maxLookDownAngle, maxLookUpAngle);

        transform.Rotate(Vector3.up * mouseX);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        

        // Jumping
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }

        // Walking and sprinting
        float moveSpeed = isSprinting ? sprintSpeed : walkSpeed;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized * moveSpeed;
        rb.MovePosition(transform.position + movement * Time.deltaTime);
        
        // Player movement
        Vector3 movementC = new Vector3(horizontalInput, 0f, verticalInput);
        movementC = transform.TransformDirection(movementC);
        transform.position += movementC * Time.deltaTime;

        // Breaking blocks
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                Block block = hit.collider.GetComponent<Block>();
                if (block != null)
                {
                    // Check if the block can be broken with the current tool (sword, pickaxe, etc.)
                    block.Break();
                }
            }
        }

        // Eating food
        if (Input.GetButtonDown("Eat"))
        {
            EatFood();
        }
    }

    private void FixedUpdate()
    {
        // Falling and fall damage
        if (rb.velocity.y < -fallDamageThreshold)
        {
            int damage = Mathf.RoundToInt(Mathf.Abs(rb.velocity.y - fallDamageThreshold));
            TakeDamage(damage);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ground detection
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Ground detection
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        // Handle damage, such as updating health UI, checking for death, etc.
    }

    private void EatFood()
    {
        // Handle eating food, such as increasing health, updating UI, etc.
    }
}
