using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]
public class InputReader : ScriptableObject, GameInput.IGameplayActions, GameInput.IMenusActions
{
    // GamePlay
    public event UnityAction<Vector2> MoveEvent = delegate { };
    public event UnityAction JumpEvent = delegate { };
    public event UnityAction JumpCancelEvent = delegate { };
    public event UnityAction RunEvent = delegate { };
    public event UnityAction RunCancelEvent = delegate { };
    public event UnityAction PullEvent = delegate { };
    public event UnityAction PullCancelEvent = delegate { };
    public event UnityAction PushEvent = delegate { };
    public event UnityAction PushCancelEvent = delegate { };
    public event UnityAction<Vector2> AimArrowEvent = delegate { };
    public event UnityAction<Vector2> AimMouseEvent = delegate { };

    // Menus
    public event UnityAction MenuPauseEvent = delegate { };
    public event UnityAction MenuCloseEvent = delegate { };
    public event UnityAction MoveSelectionEvent = delegate { };
    public event UnityAction MenuMouseMoveEvent = delegate { };

    private GameInput _gameInput;

    private void OnEnable()
    {
        if (_gameInput == null)
        {
            _gameInput = new GameInput();

            _gameInput.Gameplay.SetCallbacks(this);
            _gameInput.Menus.SetCallbacks(this);
            _gameInput.Gameplay.Enable();
        }
    }

    private void OnDisable()
    {
        DisableAllInput();
    }

    /* Menus Actions */

    public void OnMovement(InputAction.CallbackContext context)
    {
        MoveEvent.Invoke(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            JumpEvent.Invoke();
        else if (context.phase == InputActionPhase.Canceled)
            JumpCancelEvent.Invoke();
    }

    public void OnAimArrow(InputAction.CallbackContext context)
    {
        AimArrowEvent.Invoke(context.ReadValue<Vector2>());
    }
    
    public void OnAimMouse(InputAction.CallbackContext context)
    {
        AimMouseEvent.Invoke(context.ReadValue<Vector2>());
    }
    
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            RunEvent.Invoke();
        else if (context.phase == InputActionPhase.Canceled)
            RunCancelEvent.Invoke();
    }

    public void OnPull(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            PullEvent.Invoke();
        else if (context.phase == InputActionPhase.Canceled)
            PullCancelEvent.Invoke();
    }

    public void OnPush(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            PushEvent.Invoke();
        else if (context.phase == InputActionPhase.Canceled)
            PushCancelEvent.Invoke();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            MenuPauseEvent.Invoke();
    }
    

    /* Menus Actions */

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            MenuCloseEvent.Invoke();
        }
    }

    public void OnMouseMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            MenuMouseMoveEvent.Invoke();
    }

    public void OnMoveSelection(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            MoveSelectionEvent.Invoke();
    }

    public void EnableGameplayInput()
    {
        _gameInput.Menus.Disable();
        _gameInput.Gameplay.Enable();
    }

    public void DisableGameplayInput()
    {
        _gameInput.Gameplay.Disable();
    }

    public void EnableMenuInput()
    {
        _gameInput.Gameplay.Disable();
        _gameInput.Menus.Enable();
    }

    public void DisableAllInput()
    {
        _gameInput.Gameplay.Disable();
        _gameInput.Menus.Disable();
    }

    public bool GetGameplayInput()
    {
        return _gameInput.Gameplay.enabled;
    }

    public bool LeftMouseDown() => Mouse.current.leftButton.isPressed;

    public void OnPoint(InputAction.CallbackContext context)
    {
        
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        
    }
}
