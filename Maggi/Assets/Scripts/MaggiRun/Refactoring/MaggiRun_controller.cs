using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float sideSpeed = 8f;
    public float jumpForce = 20f;
    public float gravityStrength = 20f;

    private Rigidbody rb;
    private bool isGrounded = false;
    private Vector3 currentGravityDir = Vector3.down;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentGravityDir = Vector3.down;
        Physics.gravity = currentGravityDir * gravityStrength;
    }

    private void FixedUpdate()
    {

        if (Vector3.Dot(rb.linearVelocity, currentGravityDir) > 0 && !isGrounded)
        {
            rb.AddForce(currentGravityDir * gravityStrength * 1.5f);
        }
        HandleSideMovement();
    }

    private void Update()
    {
        HandleJump();
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Vector3 up = -currentGravityDir;
            rb.AddForce(up * jumpForce, ForceMode.VelocityChange);
            isGrounded = false;
        }
    }
    private void HandleSideMovement()
    {
        Vector3 right = Vector3.Cross(-currentGravityDir, transform.forward).normalized;

        if (Input.GetMouseButton(0))
            rb.MovePosition(rb.position - right * sideSpeed * Time.fixedDeltaTime);
        else if (Input.GetMouseButton(1))
            rb.MovePosition(rb.position + right * sideSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Face"))
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 normal = contact.normal;

            UpdateGravity(normal);
            isGrounded = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Face"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Face"))
        {
            isGrounded = false;
        }
    }

    private void UpdateGravity(Vector3 surfaceNormal)
    {
        Vector3 newUp = surfaceNormal.normalized;
        currentGravityDir = -newUp;
        Physics.gravity = currentGravityDir * gravityStrength;

        Quaternion targetRot = Quaternion.LookRotation(transform.forward, newUp);
        rb.MoveRotation(targetRot);
    }
}