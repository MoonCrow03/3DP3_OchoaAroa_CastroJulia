using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private InputActions _actions;

    public ButtonInputHandler Up, Down, Left, Right;

    public ButtonInputHandler LeftClick, RightClick, MiddleClick;

    public ButtonInputHandler Shift, Space, E, J, R;
    
    public float MouseScrollY { private set; get; }
    public Vector2 MovementInput { private set; get; }
    public Vector2 MouseInput { private set; get; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        if (_actions == null)
        {
            _actions = new InputActions();
            _actions.Main.Movement.performed += i => MovementInput = i.ReadValue<Vector2>();
            _actions.Main.View.performed += i => MovementInput = i.ReadValue<Vector2>();
            _actions.Main.MouseScrollY.performed += i => MouseScrollY = i.ReadValue<float>();
        }

        _actions.Enable();
    }

    private void OnDisable()
    {
        _actions.Disable();
    }

    private void Start()
    {
        Down = new ButtonInputHandler(_actions.Main.Down);
        Up = new ButtonInputHandler(_actions.Main.Up);
        Left = new ButtonInputHandler(_actions.Main.Left);
        Right = new ButtonInputHandler(_actions.Main.Right);

        Shift = new ButtonInputHandler(_actions.Main.Shift);
        Space = new ButtonInputHandler(_actions.Main.Space);

        LeftClick = new ButtonInputHandler(_actions.Main.LeftClick);
        RightClick = new ButtonInputHandler(_actions.Main.RightClick);
        MiddleClick = new ButtonInputHandler(_actions.Main.MiddleClick);
        
        E = new ButtonInputHandler(_actions.Main.E);

        J = new ButtonInputHandler(_actions.Main.J);
        R = new ButtonInputHandler(_actions.Main.R);
    }
}

public class ButtonInputHandler
{ 
    private readonly InputAction _action;
       
    private bool _tapUsed;
       
    private bool _releaseUsed;
       
    public ButtonInputHandler(InputAction action)
    {
        _action = action;
    } 
    
    public bool Hold
    {
        get
        {
            return Input();
        }
    }
       
    public bool Tap
    {
        get
        {
            var usedLastFrame = _tapUsed;
            _tapUsed = Input();
            return usedLastFrame ? false : _tapUsed;
        }
    }
       
       
    public bool Release
    {
        get
        {
            bool usedLastFrame = _releaseUsed;
            _releaseUsed = Input();
            return usedLastFrame ? !_releaseUsed : false;
        }
    }
    
    private bool Input()
    {       
        return _action.phase == InputActionPhase.Performed;
    }   

}
