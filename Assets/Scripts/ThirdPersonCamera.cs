using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -7);
    public float followSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;
    
    private float horizontalAngle = 0f;
    private float verticalAngle = 20f;
    
    void Start()
    {
        if (target != null)
        {
            Vector3 angles = transform.eulerAngles;
            horizontalAngle = angles.y;
            verticalAngle = angles.x;
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        Vector2 lookInput = PlayerInputs.Look;
        
        horizontalAngle += lookInput.x * mouseSensitivity;
        verticalAngle -= lookInput.y * mouseSensitivity;
        verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
        
        Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0);
        Vector3 rotatedOffset = rotation * offset;
        
        Vector3 desiredPosition = target.position + rotatedOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        
        transform.LookAt(target.position);
    }
}
