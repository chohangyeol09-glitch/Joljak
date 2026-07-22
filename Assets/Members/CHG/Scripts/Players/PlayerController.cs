using DefaultNamespace;
using Members.CHG.Scripts.Agents;
using Members.CHG.Scripts.Agents.FSM;
using Members.CHG.Scripts.Players.FSM;
using UnityEngine;

namespace Members.CHG.Scripts.Players
{
    public class PlayerController : Agent
    {
        [field:SerializeField] public PlayerInputSO PlayerInput { get; private set; }
        [SerializeField] public StateListSO playerStates;
        
        private StateMachine _stateMachine;
        private IControlMovement _controlMovement;
        private PlayerRotationModule _rotationModule;

        protected override void InitializeModules()
        {
            base.InitializeModules();
            _stateMachine = new StateMachine(this, playerStates.states);
            _controlMovement = GetModule<IControlMovement>();
            _rotationModule = GetModule<PlayerRotationModule>();
        }

        protected override void AfterInitializeModules()
        {
            base.AfterInitializeModules();
            //점프는 상태와 무관하게 걷는 중에도 서 있어도 되어야 해서 컨트롤러가 받는다
            PlayerInput.OnJumpKeyDown += HandleJump;

            //TODO: 임시. 공격 시스템이 생기면 그쪽에서 조준 모드를 켜야 한다
            PlayerInput.OnAttackKeyDown += HandleAimStart;
            PlayerInput.OnAttackKeyUp += HandleAimEnd;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            //PlayerInput은 ScriptableObject라 플레이가 끝나도 구독이 남는다. 반드시 해제
            PlayerInput.OnJumpKeyDown -= HandleJump;
            PlayerInput.OnAttackKeyDown -= HandleAimStart;
            PlayerInput.OnAttackKeyUp -= HandleAimEnd;
        }

        private void HandleJump() => _controlMovement.TryJump();
        private void HandleAimStart() => _rotationModule.IsAiming = true;
        private void HandleAimEnd() => _rotationModule.IsAiming = false;

        private void Start()
        {
            ChangeState(PlayerState.IDLE, transitionDuration: 0);
        }

        private void Update()
        {
            _stateMachine.UpdateMachine();
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdateMachine();
        }

        public void ChangeState(PlayerState newState, float transitionDuration)
            => _stateMachine.ChangeState((int)newState, transitionDuration);
     
        protected override void HandleHit()
        {
            
        }

        
    }
}