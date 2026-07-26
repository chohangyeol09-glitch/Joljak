using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "PlayerInput", menuName = "PlayerInput", order = 0)]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
    {
        public event Action<Vector2> OnMovementChange;
        public event Action<Vector2> OnLookChange;
        public event Action OnAttackKeyDown;
        public event Action OnAttackKeyUp;
        public event Action OnEventKeyDown;
        public event Action OnEventKeyUp;
        public event Action OnJumpKeyDown;
        public event Action OnDashKeyDown;
        public event Action OnDashKeyUp;
        
        private Controls _controls;

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
            }
            _controls.Player.Enable();
        }

        private void OnDisable()
        {
            _controls.Player.Disable();
        }
        
        
        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 movement = context.ReadValue<Vector2>();
            OnMovementChange?.Invoke(movement);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Vector2 look = context.ReadValue<Vector2>();
            OnLookChange?.Invoke(look);
        }

        public void OnAttack(InputAction.CallbackContext context)
        {

            if (context.started)
                OnAttackKeyDown?.Invoke();

            if (context.canceled)
                OnAttackKeyUp?.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started)
                OnEventKeyDown?.Invoke();
            
            if (context.canceled)
                OnEventKeyUp?.Invoke();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
                OnJumpKeyDown?.Invoke();
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnDashKeyDown?.Invoke();
            
            if (context.canceled)
                OnDashKeyUp?.Invoke();
        }
    }
}