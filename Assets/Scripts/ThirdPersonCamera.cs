using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target & Positioning")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -7);
    public Vector3 lookOffset = new Vector3(0, 1.5f, 2f); // New: offset for look target
    
    [Header("Camera Settings")]
    public float followSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;
    
    [Header("Crosshair Alignment")]
    public bool alignCrosshairWithView = true;
    public float crosshairAimHeight = 1.5f; // Height above player to aim at
    
    private float horizontalAngle = 0f;
    private float verticalAngle = 20f;

    public LayerMask notLook;
    
    void Start()
    {
        if (target != null)
        {
            Vector3 angles = transform.eulerAngles;
            horizontalAngle = angles.y;
            verticalAngle = angles.x;
        }
    }
    
    void FixedUpdate()
    {
        if (target == null) return;
        
        HandleCameraInput();
        UpdateCameraPosition();
        UpdateCameraLookDirection();
    }
    
    private void HandleCameraInput()
    {
        Vector2 lookInput = PlayerInputs.Look;
        
        horizontalAngle += lookInput.x * mouseSensitivity * Time.fixedDeltaTime;
        verticalAngle -= lookInput.y * mouseSensitivity * Time.fixedDeltaTime;
        verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
    }
    
    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0);
        Vector3 rotatedOffset = rotation * offset;
        
        Vector3 desiredPosition = target.position + rotatedOffset;
        transform.position = Vector3.Slerp(transform.position, desiredPosition, followSpeed * Time.fixedDeltaTime);
        var v = (transform.position - target.position);
        if (Physics.Raycast(target.position, v.normalized, out RaycastHit hit, v.magnitude, notLook, QueryTriggerInteraction.Ignore))
        {
            transform.position = hit.point;
        }
    }
    
    private void UpdateCameraLookDirection()
    {
        Vector3 lookTarget;
        
        if (alignCrosshairWithView)
        {
            // Calculate look target based on player's forward direction and crosshair aim height
            Vector3 playerForward = target.forward;
            Vector3 aimPoint = target.position + Vector3.up * crosshairAimHeight + playerForward * lookOffset.z;
            lookTarget = aimPoint;
        }
        else
        {
            // Traditional third-person: look at offset point relative to player
            lookTarget = target.position + lookOffset;
        }
        
        transform.LookAt(lookTarget);
    }
    
    // Helper method to get the camera's forward ray for crosshair aiming
    public Ray GetCameraRay()
    {
        Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        return Camera.main.ScreenPointToRay(screenCenter);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (target == null) return;
        
        // Draw camera offset
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(target.position + offset, 0.3f);
        Gizmos.DrawLine(target.position, target.position + offset);
        
        // Draw look target
        Vector3 lookTarget = alignCrosshairWithView ? 
            target.position + Vector3.up * crosshairAimHeight + target.forward * lookOffset.z :
            target.position + lookOffset;
            
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(lookTarget, 0.2f);
        Gizmos.DrawLine(transform.position, lookTarget);
        
        // Draw crosshair ray
        if (Application.isPlaying)
        {
            Ray ray = GetCameraRay();
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray.origin, ray.direction * 10f);
        }
    }
}
