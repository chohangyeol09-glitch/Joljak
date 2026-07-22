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
        private PlayerRotationModule _rotationModule;
        private IWeapon _weapon;

        protected override void InitializeModules()
        {
            base.InitializeModules();
            _stateMachine = new StateMachine(this, playerStates.states);
            _controlMovement = GetModule<IControlMovement>();
            _rotationModule = GetModule<PlayerRotationModule>();
            _weapon = GetModule<IWeapon>();
            
            Debug.Assert(_controlMovement != null, $"ControlMovement is null : {gameObject.name}");
            Debug.Assert(_rotationModule != null, $"RotationModule is null : {gameObject.name}");
            Debug.Assert(_weapon != null, $"Weapon is null : {gameObject.name}");
        }

        protected override void AfterInitializeModules()
        {
            base.AfterInitializeModules();
            PlayerInput.OnJumpKeyDown += HandleJump;

            PlayerInput.OnAttackKeyDown += HandleAimStart;
            PlayerInput.OnAttackKeyUp += HandleAimEnd;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
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