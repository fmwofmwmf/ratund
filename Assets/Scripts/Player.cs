using UnityEngine;

public class Player : MonoBehaviour
{
    private const float startBounciness = 1f;

    private float heft = 1f;
    private float jumpForce = 1f;
    private float moveForce = 5f;
    private float rotation_speed = 10f;

    public Rigidbody rb;
    private Camera playerCamera;
    private BoxCollider boxCollider;
    private PhysicsMaterial physicsMaterial;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        playerCamera = Camera.main;
        boxCollider = GetComponent<BoxCollider>();
        physicsMaterial = new PhysicsMaterial();
        physicsMaterial.bounceCombine = PhysicsMaterialCombine.Maximum;
        boxCollider.material = physicsMaterial;

        setBouciness(startBounciness);
    }

    void Update()
    {
        if (PlayerInputs.Jump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        Vector2 moveInput = PlayerInputs.Move;
        if (moveInput != Vector2.zero)
        {
            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 cameraRight = playerCamera.transform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = cameraForward * moveInput.y + cameraRight * moveInput.x;

            rb.AddForce(moveDirection * moveForce, ForceMode.Force);

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotation_speed * Time.deltaTime);
            }
        }
    }

    public void addHeft(float amount)
    {
        setHeft(heft + amount);
    }

    public void setHeft(float amount)
    {
        heft = amount;
    }

    public void addBoucniness(float amount)
    {
        setBouciness(physicsMaterial.bounciness + amount);
    }
    
    public void setBouciness(float amount)
    {
        if (physicsMaterial != null)
        {
            physicsMaterial.bounciness = amount;
        }
    }
}
