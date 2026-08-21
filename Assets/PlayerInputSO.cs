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
        public event Action OnAimKeyDown;
        public event Action OnAimKeyUp;
        public event Action OnInteractKeyStarted;
        public event Action OnInteractKeyPerformed;
        public event Action OnInteractKeyCanceled;
        public event Action OnJumpKeyDown;
        public event Action OnDashKeyDown;
        public event Action OnDashKeyUp;
        public event Action OnReloadKeyDown;
        public event Action OnReloadKeyUp;
        public event Action<int> OnSkillKeyDown;
        public event Action<int> OnSkillKeyUp;
        public event Action OnInventoryKeyDown;
        
        
        private Controls _controls;

        public Vector2 CurrentMovement { get; private set; }

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

        public void SetGameplayInputEnabled(bool enabled)
        {
            if (enabled)
            {
                _controls.Player.Enable();
                return;
            }
            
            _controls.Player.Disable();
            _controls.Player.Inventory.Enable();
            
            CurrentMovement = Vector2.zero;
            OnMovementChange?.Invoke(CurrentMovement);
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            CurrentMovement = context.ReadValue<Vector2>();
            OnMovementChange?.Invoke(CurrentMovement);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            OnLookChange?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnAttack(InputAction.CallbackContext context)
        {

            if (context.performed)
                OnAttackKeyDown?.Invoke();

            if (context.canceled)
                OnAttackKeyUp?.Invoke();
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnAimKeyDown?.Invoke();
            
            if (context.canceled)
                OnAimKeyUp?.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started)
                OnInteractKeyStarted?.Invoke();
            
            if (context.performed)
                OnInteractKeyPerformed?.Invoke();
            
            if (context.canceled)
                OnInteractKeyCanceled?.Invoke();
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

        public void OnReload(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnReloadKeyDown?.Invoke();
            
            if (context.canceled)
                OnReloadKeyUp?.Invoke();
                
        }

        public void OnSkill1(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnSkillKeyDown?.Invoke(0);
            
            if (context.canceled)
                OnSkillKeyUp?.Invoke(0);
        }

        public void OnSkill2(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnSkillKeyDown?.Invoke(1);
            
            if (context.canceled)
                OnSkillKeyUp?.Invoke(1);
        }

        public void OnSkill3(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnSkillKeyDown?.Invoke(2);
            
            if (context.canceled)
                OnSkillKeyUp?.Invoke(2);
        }

        public void OnSkill4(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnSkillKeyDown?.Invoke(3);
            
            if (context.canceled)
                OnSkillKeyUp?.Invoke(3);
        }

        public void OnInventory(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnInventoryKeyDown?.Invoke();
        }
    }
}