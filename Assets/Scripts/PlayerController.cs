using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpHeight = 1.5f;
    public float walkSpeed = 2f;
    public float sprintSpeed = 4f;
    public float fallDamageThreshold = 9f;
    public int startingHealth = 100;
    public Transform firstPersonCamera;

    private Rigidbody rb;
    private bool isGrounded = true;
    private int currentHealth;

    public GameObject dirtPrefab;
    public GameObject stonePrefab;
    public GameObject woodPrefab;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = startingHealth;
    }

    private void Update()
    {
        // Player movement
        float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        float horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed;
        float verticalMovement = Input.GetAxis("Vertical") * moveSpeed;

        Vector3 movement = new Vector3(horizontalMovement, 0f, verticalMovement);
        movement = Vector3.ClampMagnitude(movement, moveSpeed); // Limit diagonal movement speed

        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }

        // Falling damage
        if (transform.position.y < -fallDamageThreshold)
        {
            int fallDamage = Mathf.RoundToInt(Mathf.Abs(transform.position.y - (-fallDamageThreshold)));
            TakeDamage(fallDamage);
        }

        // Block interaction
        if (Input.GetMouseButtonDown(0))
        {
            // Left mouse button: Break block or attack with sword
            BreakBlockOrAttack();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            // Right mouse button: Place block
            PlaceBlock();
        }

        // Camera rotation
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Rotate the player left/right
        transform.Rotate(Vector3.up, mouseX);

        // Rotate the camera up/down
        Vector3 cameraRotation = firstPersonCamera.rotation.eulerAngles;
        cameraRotation.x -= mouseY;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90f, 90f);
        firstPersonCamera.rotation = Quaternion.Euler(cameraRotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void BreakBlockOrAttack()
    {
        // Raycast to detect block or enemy
        RaycastHit hit;
        if (Physics.Raycast(firstPersonCamera.position, firstPersonCamera.forward, out hit))
        {
            Block block = hit.collider.GetComponent<Block>();
            if (block != null)
            {
                // Break the block
                block.Break();
            }
            //else
            //{
                //Enemy enemy = hit.collider.GetComponent<Enemy>();
                //if (enemy != null)
                //{
                    // Attack the enemy
                    //enemy.TakeDamage(10); // Adjust damage value as needed
                //}
           // }
        }
    }

    private void PlaceBlock()
    {
        // Raycast to find the position to place the block
        RaycastHit hit;
        if (Physics.Raycast(firstPersonCamera.position, firstPersonCamera.forward, out hit))
        {
            Vector3 placePosition = hit.point + hit.normal / 2f; // Offset the block placement

            // Instantiate and place the block at the position
            GameObject blockPrefab = GetSelectedBlockPrefab(); // Implement your own method to get the selected block prefab
            Instantiate(blockPrefab, placePosition, Quaternion.identity);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Handle player death
    }
    
    private GameObject GetSelectedBlockPrefab()
    {
        GameObject selectedBlockPrefab = null;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedBlockPrefab = dirtPrefab; // Assuming dirtPrefab is a reference to the dirt block prefab
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedBlockPrefab = stonePrefab; // Assuming stonePrefab is a reference to the stone block prefab
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedBlockPrefab = woodPrefab; // Assuming woodPrefab is a reference to the wood block prefab
        }

        return selectedBlockPrefab;
    }

}

