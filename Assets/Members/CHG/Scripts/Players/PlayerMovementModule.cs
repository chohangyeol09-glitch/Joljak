using Members.CHG.Scripts.Agents.StatSystem;
using Members.CHG.Scripts.CoreSystem.ModuleSystem;
using UnityEngine;

namespace Members.CHG.Scripts.Players
{
    public class PlayerMovementModule : MonoBehaviour, IModule, IAfterInitModule, IControlMovement
    {
        [SerializeField] private StatSO moveSpeedStat;
        [SerializeField] private float gravity = -9.8f;
        [SerializeField] private float terminalVelocity = -50f;
        [SerializeField] private float groundedStickSpeed = -1.5f;


        private Vector3 _velocity;
        private float _verticalVelocity;
        private Vector3 _movementDirection;
        private CharacterController _characterController;
        private ModuleOwner _owner;
        private IStatModule _statModule;
        private Vector3 _autoVelocity;
        private float _moveSpeed;
        
        public bool CanManualMove { get; set; } = true;
        public Vector3 Velocity => _velocity;
        public bool IsGround => _characterController.isGrounded;
        
        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            _characterController = _owner.GetComponent<CharacterController>();
            _statModule = _owner.GetModule<IStatModule>();
            
            Debug.Assert(_statModule != null,$"StatModule is null : {gameObject.name}");
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
            _moveSpeed = currentValue;
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
            
            
            _velocity = CanManualMove
                ? _owner.transform.rotation * _movementDirection
                : _autoVelocity;

            _velocity *= _moveSpeed;
        }

        private void ApplyGravity()
        {
            if (IsGround && _verticalVelocity < 0)
                _verticalVelocity = groundedStickSpeed;
            else
                _verticalVelocity = Mathf.Max(
                    _verticalVelocity + gravity * Time.fixedDeltaTime, terminalVelocity);

            _velocity.y = _verticalVelocity;
        }

        private void Move()
        {
            _characterController.Move(_velocity * Time.fixedDeltaTime);
            
            
        }
        //회전 로직 따로
        
    }
}