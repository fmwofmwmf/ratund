using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    static public Player player;

    private Rigidbody rb;
    private Camera _playerCamera;
    private BoxCollider _boxCollider;
    private PhysicsMaterial _physicsMaterial;

    [Header("Chip Stack Display")]
    public Transform chipStackSpawnPoint;
    public Chip chipPrefab;
    public GameObject displayChipPrefab;
    public List<GameObject> displayChips = new List<GameObject>();

    [Header("Bounciness Settings")]
    public AnimationCurve bouncinessCurve;
    public float maxBounciness = 0.8f;
    public float maxHeft = 200f;

    [Header("Movement")]
    public float heft = 1f;
    public float jumpForce = 1f;
    public float moveForce = 5f;
    public float sprintMoveForce = 8f;
    public float rotation_speed = 10f;

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

    [Header("Jump Settings")]
    public float jumpCooldown = 0.3f; // Minimum time between jumps

    private bool _jumpPressed = false;
    private float _lastJumpTime = 0f;
    private bool _hasJumped = false; // Track if already jumped this press

    public AnimationCurve a;

    void Start()
    {
        Debug.Log(a.Evaluate(.5f));
        player = this;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        _playerCamera = Camera.main;
        _boxCollider = GetComponent<BoxCollider>();
        _physicsMaterial = new PhysicsMaterial();
        _physicsMaterial.bounceCombine = PhysicsMaterialCombine.Maximum;
        _boxCollider.material = _physicsMaterial;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        SetHeft(heft);
        
        _crosshairUI = FindFirstObjectByType<CrosshairUI>();
    }

    void Update()
    {
        HandleMovement();
        HandleInteraction();
        HandleChipDrop();
    }
    
    private void HandleMovement()
    {
        _isGrounded = CheckGrounded();
        
        // Handle jump input with single jump per press
        HandleJumpInput();

        // Declare camera directions once for the entire method
        Vector3 cameraForward = _playerCamera.transform.forward;
        Vector3 cameraRight = _playerCamera.transform.right;
        
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Handle movement input
        Vector2 moveInput = PlayerInputs.Move;
        if (moveInput != Vector2.zero)
        {
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
        
        // Player rotation follows camera direction (reuse the same cameraForward variable)
        if (cameraForward != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotation_speed * 0.5f * Time.deltaTime);
        }
    }

    private void HandleJumpInput()
    {
        bool currentJumpPressed = PlayerInputs.JumpPressed;
        bool currentJumpHeld = PlayerInputs.Jump;
        
        // Check if jump was just pressed (not held from previous frame)
        if (currentJumpPressed && !_jumpPressed)
        {
            _hasJumped = false; // Reset jump flag on new press
        }
        
        // Allow jump if: grounded, jump pressed, haven't jumped yet this press, and cooldown passed
        if (_isGrounded && currentJumpHeld && !_hasJumped && Time.time - _lastJumpTime >= jumpCooldown)
        {
            // Reset Y velocity so jump force is consistent
            Vector3 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;

            // Apply jump impulse
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            _lastJumpTime = Time.time;
            _hasJumped = true;

            Debug.Log("Jump executed!");
        }
        
        // Reset jump flag when key is released
        if (!currentJumpHeld)
        {
            _hasJumped = false;
        }
        
        _jumpPressed = currentJumpPressed;
    }

    private bool CheckGrounded()
    {
        Vector3 boxCenter = _boxCollider.bounds.center;
        Vector3 boxSize = _boxCollider.bounds.size;
        
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

    private void HandleChipDrop()
    {
        bool dropInput = PlayerInputs.Drop;
        if (dropInput)
        {
            DropChip();
        }
    }

    public Interactable GetCurrentInteractable()
    {
        return _currentInteractable;
    }
    
    public bool CanInteract()
    {
        return _currentInteractable != null;
    }

    public void PickUpChip()
    {
        Vector3 offset = new Vector3(0, 0, 0);
        for (int i = 0; i < GetChipCount(); i++)
        {
            offset.y += 0.2f;
        }

        GameObject chip = Instantiate(displayChipPrefab, chipStackSpawnPoint.position + offset, chipStackSpawnPoint.rotation, chipStackSpawnPoint);
        displayChips.Add(chip);
    }

    public int GetChipCount() 
    {
        return displayChips.Count;
    }

    public bool RemoveChip(int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            if (displayChips.Count > 0)
            {
                GameObject chipToRemove = displayChips.Last();
                displayChips.Remove(chipToRemove);
                Destroy(chipToRemove);
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public void DropChip()
    {
        bool wasRemoved = RemoveChip();
        if (!wasRemoved) return;
        Instantiate(chipPrefab, chipStackSpawnPoint.position + chipStackSpawnPoint.forward * 0.5f, chipStackSpawnPoint.rotation);
    }

    public void modifyHeft(float amount)
    {
        SetHeft(heft + amount);
    }

    public void SetHeft(float amount)
    {
        heft = amount;
        UpdateBounciness();
    }

    public void UpdateBounciness()
    { 
        float heftFraction = heft / maxHeft;
        float newBouncinessFraction = bouncinessCurve.Evaluate(heftFraction);
        float newBounciness = newBouncinessFraction * maxBounciness;

        Debug.Log("Bounciness updated to " + newBounciness + " from heft " + heft);;
        SetBounciness(newBounciness);
    }

    public void SetBounciness(float amount)
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
