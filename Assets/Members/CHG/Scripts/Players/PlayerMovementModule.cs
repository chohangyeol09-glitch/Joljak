using Members.CHG.Scripts.Agents.StatSystem;
using Members.CHG.Scripts.CoreSystem.ModuleSystem;
using UnityEngine;

namespace Members.CHG.Scripts.Players
{
    public class PlayerMovementModule : MonoBehaviour, IModule, IAfterInitModule, IControlMovement
    {
        [SerializeField] private StatSO moveSpeedStat;
        [SerializeField] private float gravity = -9.8f;
        

        private Vector3 _velocity;
        private float _verticalVelocity;
        private Vector3 _movementDirection;
        private CharacterController _characterController;
        private ModuleOwner _owner;
        private IStatModule _statModule;
        private Vector3 _autoVelocity;
        private float _moveSpeed = 8f;
        
        public bool CanManualMove { get; set; }
        public Vector3 Velocity => _velocity;
        public bool IsGround => _characterController.isGrounded;
        
        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            _characterController = _owner.GetComponent<CharacterController>();
            _statModule = _owner.GetModule<IStatModule>();
        }

        public void AfterInit()
        {
            _moveSpeed = _statModule.SubscribeStat(moveSpeedStat.AssetIndex, HandleMoveSpeedChange,
                moveSpeedStat.BaseValue);
        }

        private void OnDestroy()
        {
            _statModule?.UnSubscribeStat(moveSpeedStat.AssetIndex, HandleMoveSpeedChange);
        }

        
        
        public void SetMovementVelocity(Vector3 velocity)
        {
            _autoVelocity = velocity;
        }

        public void SetMovementDirection(Vector2 movementInput)
        {
            Vector3 newMovement = new Vector3(movementInput.x, 0, movementInput.y).normalized;
            _movementDirection = newMovement;
        }

        private void HandleMoveSpeedChange(StatSO stat, float currentValue, float prevValue)
        {
            
        }
        
        public void RotateTo(Vector3 direction)
        {
            if (direction.magnitude < Mathf.Epsilon) return;
            direction.y = 0;
            _owner.transform.forward = direction.normalized;
        }

        private void FixedUpdate()
        {
            CalculateMovement();
            ApplyGravity();
            Move();
        }

        private void CalculateMovement()
        {
            if (CanManualMove)
                _velocity = Quaternion.Euler(0, -45f, 0) * _movementDirection;
            else
                _velocity = _autoVelocity;
            
            _velocity *= _moveSpeed * Time.fixedDeltaTime;
            
        }

        private void ApplyGravity()
        {
            if (IsGround && _verticalVelocity < 0)
                _verticalVelocity = -0.03f;
            else 
                _verticalVelocity += gravity * Time.fixedDeltaTime;
            
            _velocity.y = _verticalVelocity;
        }

        private void Move()
        {
            _characterController.Move(_velocity);
        }
        //회전 로직 따로
        
    }
}