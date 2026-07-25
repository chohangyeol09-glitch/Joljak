using DefaultNamespace;
using Members.CHG.Scripts.Agents;
using Members.CHG.Scripts.Agents.FSM;
using Members.CHG.Scripts.Players.FSM;
using Members.CHG.Scripts.Weapon;
using UnityEngine;

namespace Members.CHG.Scripts.Players
{
    public class PlayerController : Agent
    {
        [field:SerializeField] public PlayerInputSO PlayerInput { get; private set; }
        [SerializeField] public StateListSO playerStates;
        
        private StateMachine _stateMachine;
        private IControlMovement _controlMovement;
        private IAimModule _aimModule;
        private IWeapon _weapon;

        protected override void InitializeModules()
        {
            base.InitializeModules();
            _stateMachine = new StateMachine(this, playerStates.states);
            _controlMovement = GetModule<IControlMovement>();
            _aimModule = GetModule<IAimModule>();
            _weapon = GetModule<IWeapon>();
            
            Debug.Assert(_controlMovement != null, $"ControlMovement is null : {gameObject.name}");
            Debug.Assert(_aimModule != null, $"AimModule is null : {gameObject.name}");
            Debug.Assert(_weapon != null, $"Weapon is null : {gameObject.name}");
        }

        protected override void AfterInitializeModules()
        {
            base.AfterInitializeModules();
            PlayerInput.OnJumpKeyDown += HandleJump;

            PlayerInput.OnAttackKeyDown += HandleAttackStart;
            PlayerInput.OnAttackKeyUp += HandleAttackEnd;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            PlayerInput.OnJumpKeyDown -= HandleJump;
            PlayerInput.OnAttackKeyDown -= HandleAttackStart;
            PlayerInput.OnAttackKeyUp -= HandleAttackEnd;
        }

        private void HandleJump() => _controlMovement.TryJump();

        //키 기반이라 여러 소스(공격, 스킬)가 겹쳐 요청해도 안전하다. 자기 키(this)만 회수
        private void HandleAttackStart()
        {
            _aimModule.RequestAim(this);
            _weapon.OnStartFire();
        }
        private void HandleAttackEnd()
        {
            _aimModule.ReleaseAim(this);
            _weapon.OnStopFire();
        }

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