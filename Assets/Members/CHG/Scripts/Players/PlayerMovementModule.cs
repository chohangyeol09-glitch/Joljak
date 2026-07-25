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
        [SerializeField] private float impulseDamping = 20f;
        [SerializeField] private float jumpSpeed = 12f;


        //수평은 Vector3(y는 항상 0), 수직은 float으로 따로 들고 마지막에만 합친다
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
        
        public bool CanManualMove { get; set; } = true;
        public Vector3 HorizontalVelocity => _horizontalVelocity;
        public float VerticalVelocity => _verticalVelocity;
        public Vector3 Velocity => _horizontalVelocity + Vector3.up * _verticalVelocity;
        public bool IsGround => _characterController.isGrounded;
        
        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            _characterController = _owner.GetComponent<CharacterController>();
            _statModule = _owner.GetModule<IStatModule>();
            _lookModule = _owner.GetModule<PlayerLookModule>();

            Debug.Assert(_statModule != null,$"StatModule is null : {gameObject.name}");
            Debug.Assert(_lookModule != null,$"LookModule is null : {gameObject.name}");
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

            //위로 띄우는 힘은 수직 채널로. 낙하 중이어도 항상 같은 높이가 나오도록 대입한다
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
            //충격량은 스스로 잦아들고, 구동력은 매 틱 만료된다. 해제 책임을 호출자에게 두지 않는다
            _impulseVelocity = Vector3.MoveTowards(
                _impulseVelocity, Vector3.zero, impulseDamping * Time.fixedDeltaTime);

            _drivenVelocity = Vector3.zero;
        }

        private void CalculateMovement()
        {
            //WASD를 카메라 yaw 기준으로 해석한다. 루트가 아니라 시선 방향이 앞이 된다
            //수동과 자동은 배타가 아니라 합쳐진다. 잠기면 수동만 0이 된다
            Vector3 manualVelocity = CanManualMove
                ? _lookModule.YawRotation * _movementDirection * _moveSpeed
                : Vector3.zero;

            _horizontalVelocity = manualVelocity + _impulseVelocity + _drivenVelocity;
            _horizontalVelocity.y = 0;
        }

        private void ApplyGravity()
        {
            if (IsGround && _verticalVelocity < 0)
                _verticalVelocity = groundedStickSpeed;
            else
                _verticalVelocity = Mathf.Max(
                    _verticalVelocity + gravity * Time.fixedDeltaTime, terminalVelocity);
        }

        private void Move()
        {
            //Velocity는 속도(m/s)라서 여기서만 변위(m)로 변환한다
            _characterController.Move(Velocity * Time.fixedDeltaTime);
        }
        //회전 로직 따로
        
    }
}