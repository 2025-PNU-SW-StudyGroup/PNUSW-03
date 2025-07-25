using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReaderMaggiRun", menuName = "Game/MaggiRun/Input Reader")]
public class InputReaderMaggiRun : ScriptableObject, GameInputMaggiRun.IGameplayActions
{
    public event UnityAction<Vector2> MoveEvent = delegate { };
    public event UnityAction JumpEvent = delegate { };
    public event UnityAction JumpCancelEvent = delegate { };
    public event UnityAction AttackEvent = delegate { };
    public event UnityAction AttackCancelEvent = delegate { };
    public event UnityAction<int> MouseStrafeEvent = delegate { };
    public Vector2 CurrentMoveValue { get; private set; } = Vector2.zero;


    private GameInputMaggiRun _gameInput;

    private void OnEnable()
    {
        if (_gameInput == null)
        {
            _gameInput = new GameInputMaggiRun();
            _gameInput.Gameplay.SetCallbacks(this);
            _gameInput.Gameplay.Enable();
        }
    }

    private void OnDisable()
    {
        DisableAllInput();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        CurrentMoveValue = value; // 현재 입력값 저장

        MoveEvent.Invoke(value);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            MouseStrafeEvent.Invoke(-1);
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            MouseStrafeEvent.Invoke(1);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed){
            JumpEvent.Invoke();
        }   
        else if (context.canceled)
            JumpCancelEvent.Invoke();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
         {
            AttackEvent.Invoke();
        }   
        else if (context.canceled)
            AttackCancelEvent.Invoke();
    }

    public void EnableGameplayInput() => _gameInput.Gameplay.Enable();
    public void DisableAllInput() => _gameInput.Gameplay.Disable();
}