using Infrastructure.Input;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Services.Scene
{
    public sealed class MobileInputService : IMobileInputService, IStartable, IDisposable
    {
        private readonly InputSystem_Actions _actions;
        private readonly JoystickInputVariable _joystickInputVar;

        public MobileInputService(InputSystem_Actions actions, JoystickInputVariable joystickInputVariable)
        {
            _actions = actions;
            _joystickInputVar = joystickInputVariable;
        }

        public void Start()
        {
            _actions.Player.Enable();

            _actions.Player.Move.performed += OnMove;
            _actions.Player.Move.canceled += OnMove;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            Vector2 value = context.ReadValue<Vector2>();
            _joystickInputVar.Value = value; 
        }

        public Vector2 GetMovement() => _joystickInputVar.Value;

        public void Dispose()
        {
            _actions.Player.Move.performed -= OnMove;
            _actions.Player.Move.canceled -= OnMove;
            _actions.Player.Disable();
        }
    }
}