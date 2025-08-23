using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour
{
    static public Player player;

    public float bounciness = 0.1f;
    public float heft = 1f;
    public float jumpForce = 1f;
    public float moveForce = 5f;
    public float sprintMoveForce = 8f;
    public float rotation_speed = 10f;

    public Rigidbody rb;
    private Camera _playerCamera;
    private BoxCollider _boxCollider;
    private PhysicsMaterial _physicsMaterial;

    [Header("Interaction")]
    private float _interactionRange = 3f;
    private LayerMask _interactionLayers = -1;
    public float raycastDistance = 10f;

    private Interactable _currentInteractable;
    private bool _interactPressed = false;
    private CrosshairUI _crosshairUI;

    [Header("Ground Detection")]
    public LayerMask groundLayerMask = -1;
    public float groundCheckDistance = 0.1f;
    public Transform groundCheckPoint;

    private bool _isGrounded = false;
    public float groundCheckRadius = 0.3f;

    public bool IsGrounded => _isGrounded;

    void Start()
    {
        player = this;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        _playerCamera = Camera.main;
        _boxCollider = GetComponent<BoxCollider>();
        _physicsMaterial = new PhysicsMaterial();
        _physicsMaterial.bounceCombine = PhysicsMaterialCombine.Maximum;
        _boxCollider.material = _physicsMaterial;
        Cursor.lockState = CursorLockMode.Locked;

        setBouciness(bounciness);
        
        _crosshairUI = FindObjectOfType<CrosshairUI>();
    }

    void Update()
    {
        HandleMovement();
        HandleInteraction();
    }
    
    private void HandleMovement()
    {
        _isGrounded = CheckGrounded();
        
        if (PlayerInputs.Jump && _isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        Vector2 moveInput = PlayerInputs.Move;
        if (moveInput != Vector2.zero)
        {
            Vector3 cameraForward = _playerCamera.transform.forward;
            Vector3 cameraRight = _playerCamera.transform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = cameraForward * moveInput.y + cameraRight * moveInput.x;

            bool sprintInput = PlayerInputs.Sprint;
            if (sprintInput == true)
            { 
                rb.AddForce(moveDirection * sprintMoveForce, ForceMode.Force);
            }
            else
            { 
                rb.AddForce(moveDirection * moveForce, ForceMode.Force);
            }
        }
        
        // Player rotation now follows camera direction instead of movement direction
        HandlePlayerRotation();
    }

    private void HandlePlayerRotation()
    {
        // Make player gradually face the camera's forward direction
        Vector3 cameraForward = _playerCamera.transform.forward;
        cameraForward.y = 0; // Keep player upright
        cameraForward.Normalize();
        
        if (cameraForward != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotation_speed * Time.deltaTime);
        }
    }

    
    private bool CheckGrounded()
    {
        Vector3 boxCenter = _boxCollider.bounds.center;
        Vector3 boxSize = _boxCollider.bounds.size;
        
        float checkDistance = groundCheckDistance + (boxSize.y * 0.5f);
        Vector3 checkPosition = new Vector3(boxCenter.x, boxCenter.y - (boxSize.y * 0.5f), boxCenter.z);
        
        bool grounded = Physics.CheckSphere(checkPosition, groundCheckRadius, groundLayerMask);
        
        return grounded;
    }

    private void FindInteractableInSight()
    {
        _currentInteractable = null;
        
        // Raycast from camera center
        Ray ray = _playerCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        RaycastHit hit;
        
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);
        
        if (Physics.Raycast(ray, out hit, raycastDistance, _interactionLayers))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            
            if (interactable != null && interactable.CanInteract)
            {
                float distance = Vector3.Distance(transform.position, hit.point);
                
                // Check if within interaction range
                if (distance <= interactable.InteractionDistance)
                {
                    _currentInteractable = interactable;
                }
            }
        }
        
        // Update crosshair visual state with interaction prompt
        if (_crosshairUI != null)
        {
            string promptText = _currentInteractable != null ? _currentInteractable.InteractionPrompt : "";
            _crosshairUI.SetInteractableState(_currentInteractable != null, promptText);
        }
    }
    
    private void HandleInteraction()
    {
        FindInteractableInSight();

        bool currentInteractInput = PlayerInputs.Interact;
        if (currentInteractInput && !_interactPressed && _currentInteractable != null)
        {
            _currentInteractable.Interact(gameObject);
        }
        _interactPressed = currentInteractInput;
    }

    public Interactable GetCurrentInteractable()
    {
        return _currentInteractable;
    }
    
    public bool CanInteract()
    {
        return _currentInteractable != null;
    }

    public void addHeft(float amount)
    {
        setHeft(heft + amount);
    }

    public void setHeft(float amount)
    {
        heft = amount;
    }

    public void addBounciness(float amount)
    {
        setBouciness(_physicsMaterial.bounciness + amount);
    }

    public void setBouciness(float amount)
    {
        if (_physicsMaterial != null)
        {
            _physicsMaterial.bounciness = amount;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _interactionRange);
        
        if (_currentInteractable != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _currentInteractable.transform.position);
        }
        
        if (_boxCollider != null)
        {
            Vector3 boxCenter = _boxCollider.bounds.center;
            Vector3 boxSize = _boxCollider.bounds.size;
            Vector3 checkPosition = new Vector3(boxCenter.x, boxCenter.y - (boxSize.y * 0.5f), boxCenter.z);
            
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
        }
    }

}
