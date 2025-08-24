using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using NUnit.Framework.Interfaces;

public class Player : MonoBehaviour
{
    static public Player player;

    private Rigidbody rb;
    private Camera _playerCamera;
    private PhysicsMaterial _physicsMaterial;

    [Header("Plinko Value")]
    public float maxPlinkoValue = 10f;

    [Header("Chip Stack Display")]
    public Transform chipStackSpawnPoint;
    public float chipHeight = 0.2f;
    public List<Chip> displayChips = new List<Chip>();
    
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
    public float airDrag;
    public float groundDrag;
    public float airSpeedModifier;
    public AnimationCurve moveDotScale;
    public Animator animator;
    public ParticleSystem particles1, particles2;

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

    public Transform hatAnchor;
    public Hat currentHat;

    void Start()
    {
        Debug.Log(a.Evaluate(.5f));
        player = this;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        _playerCamera = Camera.main;
        _physicsMaterial = new PhysicsMaterial();
        _physicsMaterial.bounceCombine = PhysicsMaterialCombine.Maximum;
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

    private void FixedUpdate()
    {
        var d = rb.linearVelocity;
        d.y = 0;
        rb.linearVelocity -= (_isGrounded? groundDrag : airDrag) * Time.fixedDeltaTime * d;
    }

    public void WearHat(Hat hat)
    {
        if (currentHat) currentHat.DropHat();
        currentHat = hat;
        hat.transform.SetParent(hatAnchor);
        hat.transform.SetPositionAndRotation(hatAnchor.position, hatAnchor.rotation);
    }

    public float getChipValue()
    {
        float result = 0f;
        foreach (Chip chip in displayChips)
        {
            result += chip.value;
        }
        return result;
    }

    public void ClearChips()
    {
        foreach (Chip chip in displayChips)
        {
            Destroy(chip.gameObject);
        }
        displayChips.Clear();
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

        bool particleOn = false;
        
        // Handle movement input
        Vector2 moveInput = PlayerInputs.Move;
        if (moveInput != Vector2.zero)
        {
            Vector3 moveDirection = cameraForward * moveInput.y + cameraRight * moveInput.x;

            bool sprintInput = PlayerInputs.Sprint;
            var d = moveDotScale.Evaluate(Vector3.Dot(moveDirection, rb.linearVelocity));
            if (sprintInput)
            {
                rb.AddForce((_isGrounded? 1 : airSpeedModifier) * sprintMoveForce * d * moveDirection, ForceMode.Force);
                animator.speed = 3;
                if (_isGrounded)
                {
                    particleOn = true;
                }
            }
            else
            {
                rb.AddForce((_isGrounded? 1 : airSpeedModifier) * moveForce * d * moveDirection, ForceMode.Force);
                animator.speed = 1;
            }
        }
        else
        {
            animator.speed = 0;
        }
        SwitchParticles(particles1, particleOn);
        SwitchParticles(particles2, particleOn);
        // Player rotation follows camera direction (reuse the same cameraForward variable)
        if (cameraForward != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotation_speed * 0.5f * Time.deltaTime);
        }
    }

    private void SwitchParticles(ParticleSystem particles, bool on)
    {
        var e = particles.emission;
        e.enabled = on;
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
        Vector3 checkPosition = transform.position + Vector3.down * groundCheckDistance;
        
        bool grounded = Physics.CheckSphere(checkPosition, groundCheckRadius, groundLayerMask);
        
        return grounded;
    }

    private void FindInteractableInSight()
    {
        
        // Raycast from camera center
        Ray ray = _playerCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        RaycastHit hit;
        
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);
        
        if (Physics.Raycast(ray, out hit, raycastDistance, _interactionLayers))
        {
            Interactable interactable = hit.collider.attachedRigidbody? hit.collider.attachedRigidbody.GetComponent<Interactable>() : hit.collider.GetComponent<Interactable>();
            
            float distance = Vector3.Distance(transform.position, hit.point);
            
            if (interactable != null && interactable.CanInteract && distance <= interactable.InteractionDistance)
            {
                if (interactable != _currentInteractable) ChangeInteractable(interactable);
            }
            else
            {
                ChangeInteractable(null);
            }
        }
        else if (_currentInteractable)
        {
            ChangeInteractable(null);
        }
        
        // Update crosshair visual state with interaction prompt
        if (_crosshairUI != null)
        {
            string promptText = _currentInteractable != null ? _currentInteractable.InteractionPrompt : "";
            _crosshairUI.SetInteractableState(_currentInteractable != null, promptText);
        }
    }

    private void ChangeInteractable(Interactable interactable)
    {
        if (_currentInteractable && _currentInteractable.toggle) _currentInteractable.toggle.enabled = false;
        _currentInteractable = interactable;
        if (!_currentInteractable)
        {
            _crosshairUI.SetInteractableState(false,"");
        } 
        else
        {
            _crosshairUI.SetInteractableState(true,interactable.InteractionPrompt);
            if (_currentInteractable.toggle) _currentInteractable.toggle.enabled = true;
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
        if (dropInput && displayChips.Count > 0)
        {
            // Work on a copy so we can clear the original list safely
            List<Chip> chipsToDrop = new List<Chip>(displayChips);
            displayChips.Clear();

            float verticalSpacing = 0.20f; // space between dropped chips
            int index = 0;

            foreach (Chip chip in chipsToDrop)
            {
                // 1. Remove from parent
                chip.transform.SetParent(null);

                // 2. Drop position (spread slightly so they don't overlap)
                Vector3 dropPosition = transform.position 
                    + transform.forward * 1.5f   // in front of player
                    + Vector3.up * (0.5f + index * verticalSpacing);


                chip.transform.position = dropPosition;

                // 3. Reset rotation to upright
                chip.transform.rotation = Quaternion.Euler(90, 0, 0);

                // 4. Re-enable physics
                Rigidbody rb = chip.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                }
                
                Collider c = chip.GetComponent<Collider>();
                if (c != null)
                {
                    c.enabled = true;
                }

                // 5. Re-enable interaction
                Interactable inter = chip.GetComponent<Interactable>();
                if (inter != null)
                {
                    inter.SetCanInteract(true);
                }

                index++;
            }
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

    private int _a;

    public float GetPlinkoValue()
    { 
        float heftFraction = heft / maxHeft;
        float newBouncinessFraction = bouncinessCurve.Evaluate(heftFraction);
        float plinkoValue = newBouncinessFraction * maxPlinkoValue;
        return plinkoValue;
    }

    public void PickUpChip(Chip chip)
    {
        Vector3 offset = new Vector3(0, 0, 0);
        for (int i = 0; i < GetChipCount(); i++)
        {
            offset.y += chipHeight;
        }

        Rigidbody rb = chip.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        Collider c = chip.GetComponent<Collider>();
        if (c != null)
        {
            c.enabled = false;
        }

        Interactable inter = chip.gameObject.GetComponent<Interactable>();
        if (inter != null)
        {
            inter.SetCanInteract(false);
        }

        _a++;

        displayChips.Add(chip);
        chip.transform.SetParent(chipStackSpawnPoint);
        chip.transform.localPosition = offset;
        chip.transform.localRotation = Quaternion.Euler(90, 0, 22.5f * (_a % 2));
    }

    public int GetChipCount() 
    {
        return displayChips.Count;
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
        
        Vector3 checkPosition = transform.position + Vector3.down * groundCheckDistance;            
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
        
    }

}
