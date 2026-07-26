using CHG.Scripts.Agents;
using CHG.Scripts.Agents.StatSystem;
using CHG.Scripts.CoreSystem.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.Players
{
    public class PlayerMovementModule : MonoBehaviour, IModule, IAfterInitModule, IControlMovement
    {
        [SerializeField] private StatSO moveSpeedStat;
        [SerializeField] private float gravity = -9.8f;
        [SerializeField] private float terminalVelocity = -50f;
        [SerializeField] private float groundedStickSpeed = -1.5f;
        [SerializeField] private float impulseDamping = 20f;
        [SerializeField] private float jumpSpeed = 12f;

        [Header("Dash")] 
        [SerializeField] private float dashSpeed = 25f;
        [SerializeField] private float dashDuration = 0.25f;
        [SerializeField] private float dashCooldown = 1.5f;
        


        private Vector3 _horizontalVelocity;
        private float _verticalVelocity;
        private Vector3 _movementDirection;
        private CharacterController _characterController;
        private ModuleOwner _owner;
        private IStatModule _statModule;
        private PlayerLookModule _lookModule;
        private Vector3 _impulseVelocity;
        private Vector3 _drivenVelocity;
        private float _moveSpeed;
        private ConstraintModule _constraint;

        public bool CanManualMove => !_constraint.Has(ConstraintType.Rooted);
        public Vector3 HorizontalVelocity => _horizontalVelocity;
        public float VerticalVelocity => _verticalVelocity;
        public Vector3 Velocity => _horizontalVelocity + Vector3.up * _verticalVelocity;
        public bool IsGround => _characterController.isGrounded;
        public float DashSpeed => dashSpeed;
        public float DashDuration => dashDuration;
        public float DashCooldown => dashCooldown;
        
        
        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            _characterController = _owner.GetComponent<CharacterController>();
            _statModule = _owner.GetModule<IStatModule>();
            _lookModule = _owner.GetModule<PlayerLookModule>();
            _constraint = _owner.GetModule<ConstraintModule>();

            Debug.Assert(_statModule != null,$"StatModule is null : {gameObject.name}");
            Debug.Assert(_lookModule != null,$"LookModule is null : {gameObject.name}");
            Debug.Assert(_constraint != null, $"ConstraintModule is null : {gameObject.name}");
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

        public void AddImpulse(Vector3 impulse)
        {
            _impulseVelocity += new Vector3(impulse.x, 0f, impulse.z);

            if (impulse.y > 0f)
                _verticalVelocity = impulse.y;
        }

        public void SetDrivenVelocity(Vector3 velocity)
        {
            _drivenVelocity = velocity;
        }

        public bool TryJump()
        {
            if (!IsGround || !CanManualMove) return false;

            AddImpulse(Vector3.up * jumpSpeed);
            return true;
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
        
        private void FixedUpdate()
        {
            CalculateMovement();
            ApplyGravity();
            Move();
            ConsumeExternalVelocity();
        }

        private void ConsumeExternalVelocity()
        {
            _impulseVelocity = Vector3.MoveTowards(
                _impulseVelocity, Vector3.zero, impulseDamping * Time.fixedDeltaTime);

            _drivenVelocity = Vector3.zero;
        }

        private void CalculateMovement()
        {
            Vector3 manualVelocity = CanManualMove
                ? _lookModule.YawRotation * _movementDirection * _moveSpeed
                : Vector3.zero;

            _horizontalVelocity = manualVelocity + _impulseVelocity + _drivenVelocity;
            _horizontalVelocity.y = 0;
        }

        private void ApplyGravity()
        {
            if (_constraint.Has(ConstraintType.Weightless))
            {
                if (IsGround && _verticalVelocity <= 0f)
                    _verticalVelocity = groundedStickSpeed;
                else if (_verticalVelocity < 0f)
                    _verticalVelocity = 0f;

                return;
            }

            if (IsGround && _verticalVelocity < 0)
                _verticalVelocity = groundedStickSpeed;
            else
                _verticalVelocity = Mathf.Max(
                    _verticalVelocity + gravity * Time.fixedDeltaTime, terminalVelocity);
        }

        private void Move()
        {
            _characterController.Move(Velocity * Time.fixedDeltaTime);
        }
    }
}