using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    [DefaultExecutionOrder(-1)] // Make sure this runs before every other script
    public class InputManager : SingletonBehavior<InputManager>
    {
        public delegate void PressedEvent();
        public event PressedEvent OnPressedEvent;
        
        public Vector2 SelectLocation => _input.PlayerActions.SelectLocation.ReadValue<Vector2>();

        private InputMaster _input;

        protected override void Awake()
        {
            base.Awake();
            _input = new InputMaster();
            Debug.Log("Input Manager setup");
        }

        private void Start()
        {
            _input.PlayerActions.Select.performed += OnPressedInput;
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Disable();
        }

        private void OnPressedInput(InputAction.CallbackContext context)
        {
            if(OnPressedEvent != null) OnPressedEvent.Invoke();
        }
    }
}