using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour
{
    static public Player player;

    public float bounciness = 1f;
    public float heft = 1f;
    public float jumpForce = 1f;
    public float moveForce = 5f;
    public float sprintMoveForce = 8f;
    public float rotation_speed = 10f;
    
    private float _interactionRange = 3f;
    private LayerMask _interactionLayers = -1;

    public Rigidbody rb;
    private Camera _playerCamera;
    private BoxCollider _boxCollider;
    private PhysicsMaterial _physicsMaterial;
    
    // Interaction variables
    private List<Interactable> _nearbyInteractables = new List<Interactable>();
    private Interactable _currentInteractable;
    private bool _interactPressed = false;

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
    }

    void Update()
    {
        HandleMovement();
        HandleInteraction();
    }
    
    private void HandleMovement()
    {
        if (PlayerInputs.Jump)
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

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotation_speed * Time.deltaTime);
            }
        }
    }
    
    private void HandleInteraction()
    {
        FindNearbyInteractables();
        
        bool currentInteractInput = PlayerInputs.Interact;
        if (currentInteractInput && !_interactPressed && _currentInteractable != null)
        {
            _currentInteractable.Interact(gameObject);
        }
        _interactPressed = currentInteractInput;
    }
    
    private void FindNearbyInteractables()
    {
        _nearbyInteractables.Clear();
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, _interactionRange, _interactionLayers);
        
        foreach (Collider col in colliders)
        {
            Interactable interactable = col.GetComponent<Interactable>();
            if (interactable != null && interactable.CanInteract)
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance <= interactable.InteractionDistance)
                {
                    _nearbyInteractables.Add(interactable);
                }
            }
        }
        
        _currentInteractable = _nearbyInteractables
            .OrderBy(i => Vector3.Distance(transform.position, i.transform.position))
            .FirstOrDefault();
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
    }
}
