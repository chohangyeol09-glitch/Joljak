using CHG.Scripts.Agents;
using CHG.Scripts.Agents.FSM;
using CHG.Scripts.Players.FSM;
using CHG.Scripts.Weapon;
using DefaultNamespace;
using Members.CHG.Scripts.Players.FSM;
using UnityEngine;

namespace CHG.Scripts.Players
{
    public class PlayerController : Agent
    {
        [field:SerializeField] public PlayerInputSO PlayerInput { get; private set; }
        [SerializeField] public StateListSO playerStates;
        
        private StateMachine _stateMachine;
        private IControlMovement _controlMovement;
        private IAimModule _aimModule;
        private IWeapon _weapon;
        private IReloadable _reloadable;
        private ConstraintModule _constraint;

        protected override void InitializeModules()
        {
            base.InitializeModules();
            _stateMachine = new StateMachine(this, playerStates.states);
            _controlMovement = GetModule<IControlMovement>();
            _aimModule = GetModule<IAimModule>();
            _weapon = GetModule<IWeapon>();
            _reloadable = _weapon as IReloadable;
            _constraint = GetModule<ConstraintModule>();
            
            Debug.Assert(_controlMovement != null, $"ControlMovement is null : {gameObject.name}");
            Debug.Assert(_aimModule != null, $"AimModule is null : {gameObject.name}");
            Debug.Assert(_weapon != null, $"Weapon is null : {gameObject.name}");
            Debug.Assert(_constraint != null, $"Constraint is null : {gameObject.name}");
        }

        protected override void AfterInitializeModules()
        {
            base.AfterInitializeModules();

            PlayerInput.OnJumpKeyDown += HandleJump;
            PlayerInput.OnAttackKeyDown += HandleAttackStart;
            PlayerInput.OnAttackKeyUp += HandleAttackEnd;
            PlayerInput.OnDashKeyDown += HandleDash;
            PlayerInput.OnReloadKeyDown += HandleReload;
            _constraint.OnConstraintAdded += HandleConstraintAdded;
            _constraint.OnConstraintRemoved += HandleConstraintRemoved;
        }

        


        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            PlayerInput.OnJumpKeyDown -= HandleJump;
            PlayerInput.OnAttackKeyDown -= HandleAttackStart;
            PlayerInput.OnAttackKeyUp -= HandleAttackEnd;
            PlayerInput.OnDashKeyDown -= HandleDash;
            PlayerInput.OnReloadKeyDown -= HandleReload;
            _constraint.OnConstraintAdded -= HandleConstraintAdded;
            _constraint.OnConstraintRemoved -= HandleConstraintRemoved;
        }

        private void HandleJump() => _controlMovement.TryJump();

        private void HandleAttackStart()
        {
            _aimModule.RequestAim(this);
            _weapon.StartFire();
        }
        private void HandleAttackEnd()
        {
            _aimModule.ReleaseAim(this);
            _weapon.StopFire();
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
        public void TryChangeState(PlayerState newState, float transitionDuration)
            => _stateMachine.TryChangeState((int)newState, transitionDuration);
        
        private void HandleDash()
        {
            TryChangeState(PlayerState.DASH, 0.05f);
        }

        private void HandleConstraintAdded(ConstraintType type)
        {
            if (type == ConstraintType.Stunned)
                TryChangeState(PlayerState.STUN, 0.1f);
        }
        
        private void HandleConstraintRemoved(ConstraintType type)
        {
            if (type != ConstraintType.Stunned) return;

            bool hasInput = PlayerInput.CurrentMovement.magnitude > 0.1f;
            ChangeState(hasInput ? PlayerState.RUN : PlayerState.IDLE, 0.1f);
        }
        
        private void HandleReload()
        {
            _reloadable?.Reload();
        }
        
        protected override void HandleHit()
        {
            
        }
    }
}