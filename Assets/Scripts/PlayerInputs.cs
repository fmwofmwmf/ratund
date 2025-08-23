using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs: MonoBehaviour
{
    public static PlayerInputs Main;
    public PlayerInput inputs;
    
    private InputAction _drop;
    public static bool Drop => Main._drop.IsPressed();
    private InputAction _interact;
    public static bool Interact => Main._interact.IsPressed();
    private InputAction _jump;
    public static bool Jump => Main._jump.ReadValue<float>() > 0;
    public static bool JumpPressed => Main._jump.WasPressedThisFrame();
    private InputAction _sprint;
    public static bool Sprint => Main._sprint.ReadValue<float>() > 0;
    private InputAction _menu;
    public static bool Menu => Main._menu.triggered;
    private InputAction _move;
    public static Vector2 Move => Main._move.ReadValue<Vector2>();
    private InputAction _look;
    public static Vector2 Look => Main._look.ReadValue<Vector2>();

    private void Awake()
    {
        if (Main == null)
        {
            Main = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(this);
    }

    private void Start()
{
    _drop = inputs.currentActionMap.FindAction("Drop");
    _interact = inputs.currentActionMap.FindAction("Interact");
    _jump = inputs.currentActionMap.FindAction("Jump");
    _sprint = inputs.currentActionMap.FindAction("Sprint");
    _menu = inputs.currentActionMap.FindAction("Menu");
    _move = inputs.currentActionMap.FindAction("Move");
    _look = inputs.currentActionMap.FindAction("Look");
}
}

